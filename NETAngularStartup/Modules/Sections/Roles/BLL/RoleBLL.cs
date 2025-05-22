using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Settings;
using DevCodeArchitect.Utilities;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing application roles, providing CRUD operations,
/// role management, and query capabilities for the role system.
/// </summary>
public class RoleBLL
{
    /// <summary>
    /// Creates a new application role if it doesn't already exist
    /// </summary>
    /// <param name="roleManager">Role manager instance</param>
    /// <param name="entity">Role data to create</param>
    /// <param name="type">Type of role being created</param>
    public static async Task Add(RoleManager<ApplicationRole>? roleManager,
                               ApplicationRole entity,
                               RoleEnum.Types type)
    {
        if (roleManager != null && !string.IsNullOrEmpty(entity.Name))
        {
            var roleExist = await roleManager.RoleExistsAsync(entity.Name);
            if (!roleExist)
            {
                var role = new ApplicationRole()
                {
                    Name = UtilityHelper.ReplaceSpaceWithHyphen(entity.Name),
                    Description = entity.Description,
                    CreatedDate = UtilityHelper.TimeZoneOffsetDateTime(),
                    IPAddress = string.Empty,
                    Type = type
                };

                await roleManager.CreateAsync(role);
            }
        }
    }

    /// <summary>
    /// Deletes a role by its ID
    /// </summary>
    /// <param name="roleManager">Role manager instance</param>
    /// <param name="roleId">ID of the role to delete</param>
    public static async Task Delete(RoleManager<ApplicationRole>? roleManager, string roleId)
    {
        if (roleManager != null && !string.IsNullOrEmpty(roleId))
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                await roleManager.DeleteAsync(role);
            }
        }
    }

    /// <summary>
    /// Loads roles based on query parameters
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>List of matching roles</returns>
    public static async Task<List<ApplicationRole>> LoadItems(ApplicationDBContext context,
                                                           RoleQueryEntity entity)
    {
        return await FetchItems(context, entity);
    }

    /// <summary>
    /// Counts roles matching query parameters
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>Count of matching records</returns>
    public static async Task<int> CountItems(ApplicationDBContext context, RoleQueryEntity entity)
    {
        return await Count(context, entity);
    }

    /// <summary>
    /// Processes batch actions on roles (currently only delete)
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="list">List of roles with actions to perform</param>
    public static async Task ProcessApiActions(ApplicationDBContext context,
                                            List<ApplicationRole> list)
    {
        foreach (var item in list)
        {
            if (!string.IsNullOrEmpty(item.Id))
            {
                switch (item.ActionStatus?.ToLower())
                {
                    case "delete":
                        await Delete(SiteConfigs.RoleManager, item.Id);
                        break;
                }
            }
        }
    }

    #region Private Helper Methods

    private static async Task<List<ApplicationRole>> FetchItems(ApplicationDBContext context,
                                                             RoleQueryEntity entity)
    {
        var query = ProcessConditions(PrepareQuery(context, entity), entity);
        return await LoadList(query, entity);
    }

    private static async Task<int> Count(ApplicationDBContext context, RoleQueryEntity entity)
    {
        return await PrepareQuery(context, entity).CountAsync();
    }

    private static Task<List<ApplicationRole>> LoadList(IQueryable<RoleQuery> query,
                                                      RoleQueryEntity entity)
    {
        return query.Select(PrepareList(entity)).ToListAsync();
    }

    private static Expression<Func<RoleQuery, ApplicationRole>> PrepareList(RoleQueryEntity entity)
    {
        return p => PrepareListItem(p, entity);
    }

    private static ApplicationRole PrepareListItem(RoleQuery p, RoleQueryEntity entity)
    {
        var record = new ApplicationRole()
        {
            Id = p.Role.Id,
            Name = p.Role.Name,
            CreatedDate = p.Role.CreatedDate,
            Type = p.Role.Type
        };

        // Additional fields for detailed views
        if (entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || !string.IsNullOrEmpty(entity.RoleID)
            || !string.IsNullOrEmpty(entity.Slug))
        {
            // Add additional profile fields here if needed
        }

        return record;
    }

    private static IQueryable<RoleQuery> ProcessConditions(IQueryable<RoleQuery> query,
                                                        RoleQueryEntity criteria)
    {
        if (!string.IsNullOrEmpty(criteria.Order))
        {
            query = (IQueryable<RoleQuery>)query.Sort(criteria.Order);
        }

        if (string.IsNullOrEmpty(criteria.RoleID))
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

    private static IQueryable<RoleQuery> PrepareQuery(ApplicationDBContext context,
                                                   RoleQueryEntity entity)
    {
        return context.AspNetRoles
            .Select(role => new RoleQuery { Role = role })
            .Where(BuildWhereClause(entity));
    }

    private static Expression<Func<RoleQuery, bool>> BuildWhereClause(RoleQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<RoleQuery>(true);

        if (!string.IsNullOrEmpty(entity.RoleID))
        {
            predicate = predicate.And(p => p.Role != null && p.Role.Id == entity.RoleID);
        }

        if (entity.AdvanceFilter)
        {
            if (!string.IsNullOrEmpty(entity.RoleName))
            {
                predicate = predicate.And(p => p.Role != null && p.Role.Name == entity.RoleName);
            }

            if (entity.Type != RoleEnum.Types.All)
            {
                predicate = predicate.And(p => p.Role != null && p.Role.Type == entity.Type);
            }

            if (!string.IsNullOrEmpty(entity.Term))
            {
                predicate = predicate.And(p => p.Role != null &&
                    (
                    (!string.IsNullOrEmpty(p.Role.Name) && p.Role.Name.Contains(entity.Term)) ||
                    p.Role.Id.ToString() == entity.Term));
            }
        }

        return predicate;
    }

    #endregion
}

/// <summary>
/// Query projection class for ApplicationRole entity
/// </summary>
public class RoleQuery
{
    public ApplicationRole? Role { get; set; }
}