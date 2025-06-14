using AngleSharp.Dom;
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
/// Business Logic Layer for handling category-related operations including
/// CRUD operations, query processing, and caching.
/// </summary>
public static class CategoriesBLL
{
    #region CRUD Operations

    /// <summary>
    /// Adds a new category to the system along with its localized data
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Category entity to add</param>
    /// <returns>The created category or null if invalid input</returns>
    public static async Task<Categories?> Add(ApplicationDBContext context, Categories entity)
    {
        if (entity.CultureCategories == null || entity.CultureCategories.Count == 0) return null;

        try
        {
            var avatarUrl = await UploadMedia.ProcessCover(
                context,
                entity.Avatar,
                entity.CultureCategories[0].Title,
                string.Empty,
                CategorySettings.AwsThumbDirname);

            var newCategory = new Categories
            {
                Term = entity.Term,
                SubTerm = entity.SubTerm,
                Avatar = avatarUrl,
                ParentId = entity.ParentId,
                Type = entity.Type,
                Priority = entity.Priority,
                IsEnabled = entity.IsEnabled,
                IsFeatured = entity.IsFeatured,
                CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                UpdatedAt = UtilityHelper.TimeZoneOffsetDateTime()
            };

            context.Entry(newCategory).State = EntityState.Added;
            await context.SaveChangesAsync();

            // process culture categories
            await CategoryDataBLL.ProcessCategoryData(context, entity.CultureCategories, newCategory.Id, false);
    
            
            return newCategory;
        }
        catch (Exception ex)
        {
            // Log error here
            return null;
        }
    }

    /// <summary>
    /// Updates an existing category and its localized data
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Category entity to update</param>
    /// <returns>The updated category or original entity if invalid</returns>
    public static async Task<Categories?> Update(ApplicationDBContext context, Categories entity)
    {
        if (entity.CategoryData == null || entity.Id <= 0)
            return entity;

        try
        {
            var existingCategory = await context.Categories
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingCategory != null)
            {
                existingCategory.Priority = entity.Priority;
                existingCategory.IsEnabled = entity.IsEnabled;
                existingCategory.UpdatedAt = UtilityHelper.TimeZoneOffsetDateTime();
                existingCategory.Avatar = await UploadMedia.ProcessCover(
                    context,
                    entity.Avatar,
                    entity.CategoryData.Title,
                    string.Empty,
                    CategorySettings.AwsThumbDirname);
                existingCategory.IsFeatured = entity.IsFeatured;

                context.Entry(existingCategory).State = EntityState.Modified;
                await context.SaveChangesAsync();

                // process culture categories
                await CategoryDataBLL.ProcessCategoryData(context, entity.CultureCategories, entity.Id, true);

            }

            return entity;
        }
        catch (Exception ex)
        {
            // Log error here
            return entity;
        }
    }

    #endregion

    #region Delete Categories

    public static async Task Delete(ApplicationDBContext context, Categories entity)
    {
        if (entity.Id <= 0) return;

        try
        {
            // Get all child categories (recursively)
            var allChildCategories = await GetAllChildCategories(context, entity.Id);
            var allCategoryIds = allChildCategories.Select(c => c.Id).Append(entity.Id).ToList();

            // Delete associated category contents for all categories
            await DeleteRange(context, allCategoryIds);

            // Delete associated category data for all categories
            await DeleteCategoryDataRange(context, allCategoryIds);

            // Delete all categories (parent and children)
            context.Categories.RemoveRange(
                context.Categories.Where(x => allCategoryIds.Contains(x.Id)));

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error here
            // Consider adding: Logger.Error($"Error deleting category {entity.Id}", ex);
            throw; // Re-throw to let caller handle the exception
        }
    }

    private static async Task<List<Categories>> GetAllChildCategories(ApplicationDBContext context, int parentId)
    {
        var childCategories = await context.Categories
            .Where(c => c.ParentId == parentId)
            .ToListAsync();

        var allChildren = new List<Categories>();

        foreach (var child in childCategories)
        {
            allChildren.Add(child);
            allChildren.AddRange(await GetAllChildCategories(context, child.Id));
        }

        return allChildren;
    }

    public static async Task DeleteRange(ApplicationDBContext context, List<int> categoryIds)
    {
        if (categoryIds == null || !categoryIds.Any()) return;

        // Implementation depends on your CategoryContentsBLL structure
        foreach (var id in categoryIds)
        {
            await CategoryContentsBLL.Delete(context, id);
        }
    }

    public static async Task DeleteCategoryDataRange(ApplicationDBContext context, List<int> categoryIds)
    {
        if (categoryIds == null || !categoryIds.Any()) return;

        // Implementation depends on your DeleteCategoryData structure
        foreach (var id in categoryIds)
        {
            await DeleteCategoryData(context, id);
        }
    }

    #endregion

    #region Content Management

    /// <summary>
    /// Deletes all localized content for a category
    /// </summary>
    private static async Task DeleteCategoryData(
        ApplicationDBContext context,
        int categoryId)
    {
        if (categoryId <= 0) return;

        try
        {
            context.CategoryData.RemoveRange(
                context.CategoryData.Where(x => x.CategoryId == categoryId));

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error here
        }
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Retrieves a single category with its localized data
    /// </summary>
    public static async Task<CategoryQuery?> GetRecord(
        ApplicationDBContext context,
        Categories entity)
    {
        if (entity?.Id <= 0) return null;

        return await context.Categories
            .Join(context.CategoryData,
                category => category.Id,
                data => data.CategoryId,
                (category, data) => new CategoryQuery
                {
                    Category = category,
                    Data = data
                })
            .Where(p => p.Category != null && p.Category.Id == entity.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Loads categories based on query parameters with optional caching
    /// </summary>
    public static async Task<List<Categories>?> LoadItems(
        ApplicationDBContext context,
        CategoryQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await FetchItems(context, entity);
        }

        var cacheKey = GenerateCacheKey("lng_category", entity);
        var cachedData = SiteConfigs.Cache?.Get<List<Categories>>(cacheKey);

        if (cachedData != null) return cachedData;

        var data = await FetchItems(context, entity);

        if (SiteConfigs.Cache != null && data != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(ApplicationSettings.Caching.Duration));

            SiteConfigs.Cache.Set(cacheKey, data, cacheOptions);
        }

        return data;
    }

    /// <summary>
    /// Counts categories matching query parameters with optional caching
    /// </summary>
    public static async Task<int> CountItems(
        ApplicationDBContext context,
        CategoryQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await Count(context, entity);
        }

        var cacheKey = GenerateCacheKey("cnt_category", entity);

        if (SiteConfigs.Cache != null &&
            SiteConfigs.Cache.TryGetValue<int>(cacheKey, out var cachedCount))
        {
            return cachedCount;
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

    #region Utility Methods

    /// <summary>
    /// Generates a unique slug for a category
    /// </summary>
    public static async Task<string> GenerateSlug(
        ApplicationDBContext context,
        string? value,
        int length,
        bool generateUniqueSlug = false)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var slug = UtilityHelper.PrepareSlug(value, length);
        if (!generateUniqueSlug) return slug;

        // Check for existing slug
        var existingCount = await Count(context, new CategoryQueryEntity
        {
            Slug = slug,
            AdvanceFilter = false
        });

        if (existingCount == 0) return slug;

        // Handle duplicate slugs
        var totalOccurrences = await Count(context, new CategoryQueryEntity
        {
            SlugStartedWith = slug,
            AdvanceFilter = false
        });

        var newSlug = $"{slug}-{totalOccurrences + 1}";

        // Verify new slug is unique
        existingCount = await Count(context, new CategoryQueryEntity
        {
            Slug = newSlug,
            AdvanceFilter = false
        });

        return existingCount > 0
            ? $"{newSlug}-{DateTime.Now.Ticks.ToString().Substring(0, 10)}"
            : newSlug;
    }

    /// <summary>
    /// Processes batch actions on multiple categories
    /// </summary>
    public static async Task ProcessApiActions(
        ApplicationDBContext context,
        List<Categories> list)
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

                case "delete":
                    await Delete(context, item);
                    break;
            }
        }
    }

    #endregion

    #region Private Helpers

    private static async Task<List<Categories>> FetchItems(
        ApplicationDBContext context,
        CategoryQueryEntity entity)
    {
        var query = ProcessConditions(PrepareQuery(context, entity), entity);
        return await LoadList(query, entity);
    }

    private static async Task<int> Count(
        ApplicationDBContext context,
        CategoryQueryEntity entity)
    {
        return await PrepareQuery(context, entity).CountAsync();
    }

    private static Task<List<Categories>> LoadList(IQueryable<CategoryQuery> query, CategoryQueryEntity entity)
    {
        return query.Select(PrepareList(entity)).ToListAsync();
    }
    private static System.Linq.Expressions.Expression<Func<CategoryQuery, Categories>> PrepareList(CategoryQueryEntity entity)
    {
        return p => prepareListItem(p, entity);
    }

    private static Categories prepareListItem(CategoryQuery p, CategoryQueryEntity entity)
    {
        var record = new Categories()
        {
            Id = p.Category.Id,
            Term = p.Category.Term,
            SubTerm = p.Category.SubTerm,
            Records = p.Category.Records,
            ParentId = p.Category.ParentId,
            UpdatedAt = p.Category.UpdatedAt
        };

        // fields will be available if 
        // list view or profile view
        // id > 0
        // slug which is equivalent with id != ""
        if (entity.ColumnOptions == Types.FetchColumnOptions.List
            || entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || entity.Id > 0
            || !string.IsNullOrEmpty(entity.Slug))
        {
            record.Priority = p.Category.Priority;
            record.IsEnabled = p.Category.IsEnabled;
            record.IsFeatured = p.Category.IsFeatured;
            record.Avatar = p.Category.Avatar;
        }

        // this will return default thumb or cover url if user not updated avatar, cover yet
        record.Avatar = UploadMedia.GetImageUrl(p.Category.Avatar, CategorySettings.DefaultThumbnail);

        // profile view
        if (entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || entity.Id > 0
            || !string.IsNullOrEmpty(entity.Slug))
        {
            //record.description = p.category.description;
        }

        if (p.Data != null)
        {
            record.CategoryData = new CategoryData()
            {
                Id = p.Data.Id,
                Title = p.Data.Title,
                SubTitle = p.Data.SubTitle,
                Culture = p.Data.Culture,
                Description = p.Data.Description
            };
        }

        return record;
    }


    public static IQueryable<CategoryQuery> ProcessConditions(IQueryable<CategoryQuery> collectionQuery, CategoryQueryEntity query)
    {
        if (!string.IsNullOrEmpty(query.Order))
            collectionQuery = (IQueryable<CategoryQuery>)collectionQuery.Sort(query.Order);

        if (query.Id == 0)
        {
            // skip logic
            if (query.PageNumber > 1)
                collectionQuery = collectionQuery.Skip(query.PageSize * (query.PageNumber - 1));
            // take logic
            if (!query.LoadAll)
                collectionQuery = collectionQuery.Take(query.PageSize);
        }

        return collectionQuery;
    }

    private static IQueryable<CategoryQuery> PrepareQuery(
        ApplicationDBContext context,
        CategoryQueryEntity entity)
    {
        return context.Categories
            .Join(context.CategoryData,
                category => category.Id,
                data => data.CategoryId,
                (category, data) => new CategoryQuery
                {
                    Category = category,
                    Data = data
                })
            .Where(ReturnWhereClause(entity));
    }

    private static Expression<Func<CategoryQuery, bool>> ReturnWhereClause(
        CategoryQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<CategoryQuery>(true);

        // Set default culture if not specified
        entity.Culture ??= "en";

        predicate = predicate.And(p => p.Data != null && p.Data.Culture == entity.Culture);

        if (entity.Id > 0)
        {
            predicate = predicate.And(p => p.Category != null && p.Category.Id == entity.Id);
        }
        else if (!string.IsNullOrEmpty(entity.Slug))
        {
            predicate = predicate.And(p => p.Category != null &&
                (p.Category.Term == entity.Slug || p.Category.SubTerm == entity.Slug));
        }
        else if (!string.IsNullOrEmpty(entity.SlugStartedWith))
        {
            predicate = predicate.And(p => p.Category != null &&
                ((!string.IsNullOrEmpty(p.Category.Term) &&
                  p.Category.Term.StartsWith(entity.SlugStartedWith)) ||
                 (!string.IsNullOrEmpty(p.Category.SubTerm) &&
                  p.Category.SubTerm.StartsWith(entity.SlugStartedWith))));
        }
        else if (entity.AdvanceFilter)
        {
            // Add advanced filtering logic here
            // categories must be filter by type
            predicate = predicate.And(p => p.Category != null && p.Category.Type == entity.type);

            if (!string.IsNullOrEmpty(entity.start_search_key))
                predicate = predicate.And(p => 
                (p.Data != null && !string.IsNullOrEmpty(p.Data.Title) && p.Data.Title.StartsWith(entity.start_search_key) ||
                (p.Category != null && !string.IsNullOrEmpty(p.Category.Term) && p.Category.Term.StartsWith(entity.start_search_key))));

            if (entity.isfeatured != Types.FeaturedTypes.All)
                predicate = predicate.And(p => p.Category != null && p.Category.IsFeatured == entity.isfeatured);

            if (entity.parentid > -1)
                predicate = predicate.And(p => p.Category != null && p.Category.ParentId == entity.parentid);

            if (entity.IsEnabled != Types.ActionTypes.All)
                predicate = predicate.And(p => p.Category != null && p.Category.IsEnabled == entity.IsEnabled);

            // broad search
            if (!string.IsNullOrEmpty(entity.Term))
            {
                predicate = predicate.And(p =>
                 (p.Data != null && !string.IsNullOrEmpty(p.Data.Title) && p.Data.Title.Contains(entity.Term) ||
                 (p.Data != null && !string.IsNullOrEmpty(p.Data.SubTitle) && p.Data.SubTitle.Contains(entity.Term) ||
                 (p.Data != null && !string.IsNullOrEmpty(p.Data.Description) && p.Data.Description.Contains(entity.Term) ||
                 (p.Category != null && p.Category.Id.ToString() == entity.Term)))));
            }
            
        }

        return predicate;
    }

    private static async Task UpdateField(
        ApplicationDBContext context,
        int id,
        dynamic value,
        string fieldName)
    {
        if (id <= 0) return;

        var item = await context.Categories
            .FirstOrDefaultAsync(p => p.Id == id);

        if (item == null) return;

        var property = item.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

        if (property != null)
        {
            property.SetValue(item, value);
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }

    private static string GenerateCacheKey(string prefix, CategoryQueryEntity entity)
    {
        var builder = new StringBuilder()
            .Append(prefix)
            .Append('_')
            .Append(entity.PageNumber)
            .Append(entity.type)
            .Append(entity.PageSize)
            .Append(entity.parentid)
            .Append(entity.Culture)
            .Append(entity.LoadAll);

        if (!string.IsNullOrEmpty(entity.Order))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(
                entity.Order.ToLower()));
        }

        return builder.ToString();
    }

    #endregion
}

/// <summary>
/// Represents a joined result of a category and its localized data
/// </summary>
public class CategoryQuery
{
    public Categories? Category { get; set; }
    public CategoryData? Data { get; set; }
}