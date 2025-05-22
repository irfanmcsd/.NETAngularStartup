using DevCodeArchitect.DBContext;
using DevCodeArchitect.Utilities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for ErrorLogs entity providing CRUD operations,
/// query capabilities, and bulk actions for error logging system.
/// </summary>
public class ErrorLogsBLL
{
    /// <summary>
    /// Adds a new error log entry to the database
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Error log data to add</param>
    /// <returns>The created error log with generated ID</returns>
    public static async Task<ErrorLogs> Add(ApplicationDBContext context, ErrorLogs entity)
    {
        var newErrorLog = new ErrorLogs()
        {
            Description = entity.Description,
            Url = entity.Url,
            StackTrace = entity.StackTrace,
            CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
        };

        context.Entry(newErrorLog).State = EntityState.Added;
        await context.SaveChangesAsync();

        return newErrorLog;
    }

    /// <summary>
    /// Deletes a specific error log entry
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Error log to delete (must have valid ID)</param>
    public static async Task Delete(ApplicationDBContext context, ErrorLogs entity)
    {
        if (entity.Id > 0)
        {
            context.ErrorLogs.RemoveRange(context.ErrorLogs.Where(x => x.Id == entity.Id));
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Deletes all error log entries from the database
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task DeleteAll(ApplicationDBContext context)
    {
        var allLogs = from c in context.ErrorLogs select c;
        context.ErrorLogs.RemoveRange(allLogs);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets a single error log record by ID
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Error log query with ID to fetch</param>
    /// <returns>The matching error log or null if not found</returns>
    public static async Task<ErrorLogs?> GetRecord(ApplicationDBContext context, ErrorLogs entity)
    {
        return await context.ErrorLogs
            .Where(p => p.Id == entity.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Loads error logs based on query parameters
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>List of matching error logs</returns>
    public static async Task<List<ErrorLogs>> LoadItems(ApplicationDBContext context, ErrorLogQueryEntity entity)
    {
        return await FetchItems(context, entity);
    }

    /// <summary>
    /// Counts error logs matching query parameters
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>Count of matching records</returns>
    public static async Task<int> CountItems(ApplicationDBContext context, ErrorLogQueryEntity entity)
    {
        return await PrepareQuery(context, entity).CountAsync();
    }

    /// <summary>
    /// Processes batch actions on error logs (delete, delete all)
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="list">List of error logs with actions to perform</param>
    public static async Task ProcessApiActions(ApplicationDBContext context, List<ErrorLogs> list)
    {
        foreach (var item in list)
        {
            if (item.Id > 0)
            {
                switch (item.ActionStatus?.ToLower())
                {
                    case "delete":
                        await Delete(context, item);
                        break;

                    case "deleteall":
                        await DeleteAll(context);
                        break;
                }
            }
        }
    }

    #region Private Helper Methods

    private static async Task<List<ErrorLogs>> FetchItems(ApplicationDBContext context, ErrorLogQueryEntity entity)
    {
        var query = ProcessConditions(PrepareQuery(context, entity), entity);
        return await LoadList(query, entity);
    }

    private static Task<List<ErrorLogs>> LoadList(IQueryable<ErrorLogQuery> query, ErrorLogQueryEntity entity)
    {
        return query.Select(PrepareList(entity)).ToListAsync();
    }

    private static Expression<Func<ErrorLogQuery, ErrorLogs>> PrepareList(ErrorLogQueryEntity entity)
    {
        return p => PrepareListItem(p, entity);
    }

    private static ErrorLogs PrepareListItem(ErrorLogQuery p, ErrorLogQueryEntity entity)
    {
        var record = new ErrorLogs()
        {
            Id = p.Log.Id,
            Description = p.Log.Description,
            Url = p.Log.Url,
            CreatedAt = p.Log.CreatedAt,
        };

        // Include stack trace for detailed views
        if (entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || entity.Id > 0
            || !string.IsNullOrEmpty(entity.Slug))
        {
            record.StackTrace = p.Log.StackTrace;
        }

        return record;
    }

    private static IQueryable<ErrorLogQuery> ProcessConditions(IQueryable<ErrorLogQuery> query, ErrorLogQueryEntity criteria)
    {
        if (!string.IsNullOrEmpty(criteria.Order))
        {
            query = (IQueryable<ErrorLogQuery>)query.Sort(criteria.Order);
        }

        if (criteria.Id == 0)
        {
            // Apply pagination
            if (criteria.PageNumber > 1)
            {
                query = query.Skip(criteria.PageSize * (criteria.PageNumber - 1));
            }

            if (!criteria.LoadAll)
            {
                query = query.Take(criteria.PageSize);
            }
        }

        return query;
    }

    private static IQueryable<ErrorLogQuery> PrepareQuery(ApplicationDBContext context, ErrorLogQueryEntity entity)
    {
        return context.ErrorLogs
            .Select(log => new ErrorLogQuery { Log = log })
            .Where(BuildWhereClause(entity));
    }

    private static Expression<Func<ErrorLogQuery, bool>> BuildWhereClause(ErrorLogQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<ErrorLogQuery>(true);

        if (entity.Id > 0)
        {
            predicate = predicate.And(p => p.Log != null && p.Log.Id == entity.Id);
        }

        if (entity.AdvanceFilter && !string.IsNullOrEmpty(entity.Term))
        {
            predicate = predicate.And(p => p.Log != null &&
                (
                (!string.IsNullOrEmpty(p.Log.Url) && p.Log.Url.Contains(entity.Term)) ||
                (!string.IsNullOrEmpty(p.Log.Description) && p.Log.Description.Contains(entity.Term)) ||
                (!string.IsNullOrEmpty(p.Log.Description) && p.Log.Description.Contains(entity.Term)) ||
                 p.Log.StackTrace.Contains(entity.Term)));
        }

        return predicate;
    }

    #endregion
}

/// <summary>
/// Query projection class for ErrorLogs entity
/// </summary>
public class ErrorLogQuery
{
    public ErrorLogs? Log { get; set; }
}
