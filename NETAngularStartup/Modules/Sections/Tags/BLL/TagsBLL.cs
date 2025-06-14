using DevCodeArchitect.DBContext;
using DevCodeArchitect.Settings;
using DevCodeArchitect.Utilities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Text;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing tags with CRUD operations, query capabilities,
/// and caching support. Handles tag processing, search queries, and bulk actions.
/// </summary>
public class TagsBLL
{
    #region CRUD Operations

    /// <summary>
    /// Adds a new tag to the database
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Tag data to add</param>
    /// <returns>The created tag with generated ID</returns>
    public static async Task<Tags> Add(ApplicationDBContext context, Tags entity)
    {
        var newTag = new Tags()
        {
            Title = entity.Title,
            Term = entity.Term,
            Type = entity.Type,
            TagType = entity.TagType,
            IsEnabled = entity.IsEnabled
        };

        context.Entry(newTag).State = EntityState.Added;
        await context.SaveChangesAsync();

        return newTag;
    }

    /// <summary>
    /// Updates an existing tag
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Tag data to update</param>
    public static async Task Update(ApplicationDBContext context, Tags entity)
    {
        if (entity.Id > 0)
        {
            var existingTag = await context.Tags
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingTag != null)
            {
                existingTag.Title = entity.Title;
                existingTag.TagLevel = entity.TagLevel;
                existingTag.TagType = entity.TagType;

                context.Entry(existingTag).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Deletes a tag by ID
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Tag to delete (must have valid ID)</param>
    public static async Task Delete(ApplicationDBContext context, Tags entity)
    {
        if (entity.Id > 0)
        {
            context.Tags.RemoveRange(context.Tags.Where(x => x.Id == entity.Id));
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Gets a single tag record by ID
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Tag with ID to fetch</param>
    /// <returns>The matching tag or null if not found</returns>
    public static async Task<Tags?> GetRecord(ApplicationDBContext context, Tags entity)
    {
        return await context.Tags
            .FirstOrDefaultAsync(p => p.Id == entity.Id);
    }

    /// <summary>
    /// Updates a specific field of a tag record
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="id">ID of the tag to update</param>
    /// <param name="value">New value for the field</param>
    /// <param name="fieldName">Name of the field to update</param>
    public static async Task UpdateField(ApplicationDBContext context,
                                      int id,
                                      dynamic value,
                                      string fieldName)
    {
        if (id > 0)
        {
            var item = await context.Tags.FirstOrDefaultAsync(p => p.Id == id);
            if (item != null)
            {
                var property = item.GetType().GetProperty(fieldName,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);

                if (property != null && property.CanWrite)
                {
                    property.SetValue(item, value);
                    context.Entry(item).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Loads tags based on query parameters with optional caching
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>List of matching tags</returns>
    public static async Task<List<Tags>> LoadItems(ApplicationDBContext context,
                                                TagQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await FetchItems(context, entity);
        }

        var cacheKey = GenerateCacheKey("lng_tags", entity);
        var cachedData = SiteConfigs.Cache?.Get<List<Tags>>(cacheKey);

        if (cachedData != null)
        {
            return cachedData;
        }

        var data = await FetchItems(context, entity);

        if (SiteConfigs.Cache != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(ApplicationSettings.Caching.Duration));

            SiteConfigs.Cache.Set(cacheKey, data, cacheOptions);
        }

        return data;
    }

    /// <summary>
    /// Counts tags matching query parameters with optional caching
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters</param>
    /// <returns>Count of matching records</returns>
    public static async Task<int> CountItems(ApplicationDBContext context,
                                          TagQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await Count(context, entity);
        }

        var cacheKey = GenerateCacheKey("cnt_tag", entity);
        var cachedCount = SiteConfigs.Cache?.Get<int?>(cacheKey);

        if (cachedCount.HasValue)
        {
            return cachedCount.Value;
        }

        var count = await Count(context, entity);

        if (SiteConfigs.Cache != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(ApplicationSettings.Caching.Duration));

            SiteConfigs.Cache.Set(cacheKey, count, cacheOptions);
        }

        return count;
    }

    #endregion

    #region Tag Processing

    /// <summary>
    /// Processes a comma-separated list of tags, adding new ones that don't exist
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="tags">Comma-separated tag string</param>
    /// <param name="type">Type of tags to process</param>
    /// <param name="tagType">Tag type to assign</param>
    public static async Task ProcessTags(ApplicationDBContext context,
                                       string tags,
                                       TagEnum.Types type,
                                       TagEnum.TagType tagType)
    {
        if (string.IsNullOrEmpty(tags)) return;

        var tagItems = tags.Split(',');
        foreach (var tag in tagItems)
        {
            if (tag.Length >= 3 && tag.Length <= 40)
            {
                var normalizedTag = UtilityHelper.ReplaceSpaceWithHyphen(tag.ToLower().Trim());

                var exists = await Count(context, new TagQueryEntity()
                {
                    Term = normalizedTag,
                    TagLevel = TagEnum.TagLevel.All,
                    Type = type,
                    IsEnabled = Types.ActionTypes.All,
                    TagType = tagType
                }) > 0;

                if (!exists)
                {
                    await Add(context, new Tags()
                    {
                        Title = tag.Trim(),
                        Term = normalizedTag,
                        Type = type,
                        TagType = tagType,
                        IsEnabled = Types.ActionTypes.Enabled
                    });
                }
            }
        }
    }

    /// <summary>
    /// Records a search query as a tag for analytics purposes
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="term">Search term to record</param>
    /// <param name="type">Type of search</param>
    public static async Task AddSearchQuery(ApplicationDBContext context,
                                         string? term,
                                         TagEnum.Types type)
    {
        if (string.IsNullOrWhiteSpace(term)) return;
        if (term.Length < 3 || term.Length > 40 || term.Contains("@")) return;

        await Add(context, new Tags()
        {
            Title = term.Trim(),
            Term = UtilityHelper.ReplaceSpaceWithHyphen(term.Trim()),
            TagType = TagEnum.TagType.UserSearches,
            Type = type,
            IsEnabled = Types.ActionTypes.Enabled
        });
    }

    #endregion

    #region Bulk Actions

    /// <summary>
    /// Processes batch actions on tags (enable/disable, change level, delete)
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="list">List of tags with actions to perform</param>
    public static async Task ProcessApiActions(ApplicationDBContext context,
                                            List<Tags> list)
    {
        foreach (var item in list)
        {
            if (item.Id <= 0) continue;

            switch (item.ActionStatus?.ToLower())
            {
                case "enable":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Enabled, "IsEnabled");
                    break;

                case "disable":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Disabled, "IsEnabled");
                    break;

                case "high":
                    await UpdateField(context, item.Id, (byte)TagEnum.TagLevel.High, "TagLevel");
                    break;

                case "medium":
                    await UpdateField(context, item.Id, (byte)TagEnum.TagLevel.Medium, "TagLevel");
                    break;

                case "low":
                    await UpdateField(context, item.Id, (byte)TagEnum.TagLevel.Low, "TagLevel");
                    break;

                case "delete":
                    await Delete(context, item);
                    break;
            }
        }
    }

    #endregion

    #region Private Helper Methods

    private static async Task<List<Tags>> FetchItems(ApplicationDBContext context,
                                                  TagQueryEntity entity)
    {
        var query = ProcessConditions(PrepareQuery(context, entity), entity);
        return await LoadList(query, entity);
    }

    private static async Task<int> Count(ApplicationDBContext context,
                                      TagQueryEntity entity)
    {
        return await PrepareQuery(context, entity).CountAsync();
    }

    private static Task<List<Tags>> LoadList(IQueryable<TagsQuery> query,
                                          TagQueryEntity entity)
    {
        return query.Select(PrepareList(entity)).ToListAsync();
    }

    private static Expression<Func<TagsQuery, Tags>> PrepareList(TagQueryEntity entity)
    {
        return p => PrepareListItem(p, entity);
    }

    private static Tags PrepareListItem(TagsQuery p, TagQueryEntity entity)
    {
        var record = new Tags()
        {
            Id = p.Tag.Id,
            Title = p.Tag.Title,
            Term = p.Tag.Term,
            Records = p.Tag.Records,
        };

        // Include additional fields for list/profile views
        if (entity.ColumnOptions == Types.FetchColumnOptions.List ||
            entity.ColumnOptions == Types.FetchColumnOptions.Profile ||
            entity.Id > 0 ||
            !string.IsNullOrEmpty(entity.Slug))
        {
            record.TagLevel = p.Tag.TagLevel;
            record.TagType = p.Tag.TagType;
            record.IsEnabled = p.Tag.IsEnabled;
        }

        return record;
    }

    private static IQueryable<TagsQuery> ProcessConditions(IQueryable<TagsQuery> query,
                                                        TagQueryEntity criteria)
    {
        if (!string.IsNullOrEmpty(criteria.Order))
        {
            query = (IQueryable<TagsQuery>)query.Sort(criteria.Order);
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

    private static IQueryable<TagsQuery> PrepareQuery(ApplicationDBContext context,
                                                   TagQueryEntity entity)
    {
        return context.Tags
            .Select(tag => new TagsQuery { Tag = tag })
            .Where(BuildWhereClause(entity));
    }

    private static Expression<Func<TagsQuery, bool>> BuildWhereClause(TagQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<TagsQuery>(true);

        if (entity.Id > 0)
        {
            predicate = predicate.And(p => p.Tag != null && p.Tag.Id == entity.Id);
        }

        if (entity.AdvanceFilter)
        {
            if (!string.IsNullOrEmpty(entity.StartSearchKey))
            {
                predicate = predicate.And(p => p.Tag != null &&
                    (p.Tag.Title.StartsWith(entity.StartSearchKey) ||
                     p.Tag.Term.StartsWith(entity.StartSearchKey)));
            }

            if (!string.IsNullOrEmpty(entity.SlugStartedWith))
            {
                predicate = predicate.And(p => p.Tag != null &&
                    p.Tag.Term.StartsWith(entity.SlugStartedWith));
            }

            if (entity.Type != TagEnum.Types.All)
            {
                predicate = predicate.And(p => p.Tag != null &&
                    p.Tag.Type == entity.Type);
            }

            if (entity.TagLevel != TagEnum.TagLevel.All)
            {
                predicate = predicate.And(p => p.Tag != null &&
                    p.Tag.TagLevel == entity.TagLevel);
            }

            if (entity.TagType != TagEnum.TagType.All)
            {
                predicate = predicate.And(p => p.Tag != null &&
                    p.Tag.TagType == entity.TagType);
            }

            if (entity.IsEnabled != Types.ActionTypes.All)
            {
                predicate = predicate.And(p => p.Tag != null &&
                    p.Tag.IsEnabled == entity.IsEnabled);
            }

            if (!string.IsNullOrEmpty(entity.Term))
            {
                predicate = predicate.And(p => p.Tag != null &&
                    (p.Tag.Title.Contains(entity.Term) ||
                     p.Tag.Id.ToString() == entity.Term));
            }
        }

        return predicate;
    }

    private static string GenerateCacheKey(string prefix, TagQueryEntity entity)
    {
        var builder = new StringBuilder();
        builder.Append($"{prefix}_{entity.PageNumber}{entity.Type}{entity.PageSize}{entity.TagType}{entity.TagLevel}");

        if (!string.IsNullOrEmpty(entity.Order))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(entity.Order.ToLower()));
        }

        return builder.ToString();
    }

    #endregion
}

/// <summary>
/// Query projection class for Tags entity
/// </summary>
public class TagsQuery
{
    public Tags? Tag { get; set; }
}