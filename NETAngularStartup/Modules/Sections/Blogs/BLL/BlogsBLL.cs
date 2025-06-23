using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Settings;
using DevCodeArchitect.Utilities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for managing blog posts and related operations
/// </summary>
public static class BlogsBLL
{
    #region CRUD Operations

    /// <summary>
    /// Adds a new blog post to the system
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Blog entity to add</param>
    /// <returns>The created blog post or null if invalid input</returns>
    public static async Task<Blogs?> Add(ApplicationDBContext context, Blogs entity)
    {
        if (entity.BlogCultureData == null || entity.BlogCultureData.Count == 0) return null;

        try
        {
            var coverUrl = await UploadMedia.ProcessCover(
                context,
                entity.Cover,
                entity.BlogCultureData[0].Title,
                entity.UserId,
                BlogSettings.AwsCoverDirname);

            var newBlog = new Blogs
            {
                UserId = entity.UserId,
                Term = entity.Term,
                Tags = entity.Tags,
                IsEnabled = entity.IsEnabled,
                IsApproved = entity.IsApproved,
                IsDraft = entity.IsDraft,
                CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                UpdatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                Cover = coverUrl
            };

            context.Entry(newBlog).State = EntityState.Added;
            await context.SaveChangesAsync();

            // process culture data
            await BlogDataBLL.ProcessBlogData(context, entity.BlogCultureData, newBlog.Id, false);

            await CategoryContentsBLL.ProcessAssociatedContentCategories(
                context,
                entity.Categories,
                newBlog.Id,
                CategoryEnum.Types.Blogs,
                isUpdate: false);

            return newBlog;
        }
        catch (Exception ex)
        {
            // Log error here
            return null;
        }
    }

    /// <summary>
    /// Updates an existing blog post
    /// </summary>
    public static async Task<Blogs?> Update(ApplicationDBContext context, Blogs entity)
    {
        if (entity.BlogData == null || entity.Id <= 0) return null;

        try
        {
            var existingBlog = await context.Blogs
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingBlog != null)
            {
                existingBlog.Cover = await UploadMedia.ProcessCover(
                    context,
                    entity.Cover,
                    entity.BlogData.Title,
                    entity.UserId,
                    BlogSettings.AwsCoverDirname);

                existingBlog.Tags = entity.Tags;
                existingBlog.UpdatedAt = UtilityHelper.TimeZoneOffsetDateTime();

                context.Entry(existingBlog).State = EntityState.Modified;
                await context.SaveChangesAsync();

                // process culture data
                await BlogDataBLL.ProcessBlogData(context, entity.BlogCultureData, existingBlog.Id, true);
                await CategoryContentsBLL.ProcessAssociatedContentCategories(
                    context,
                    entity.Categories,
                    existingBlog.Id,
                    CategoryEnum.Types.Blogs,
                    isUpdate: true);
            }

            return existingBlog;
        }
        catch (Exception ex)
        {
            // Log error here
        }
        return null;
    }

    /// <summary>
    /// Deletes a blog post
    /// </summary>
    public static async Task Delete(ApplicationDBContext context, Blogs entity)
    {
        if (entity.Id <= 0) return;

        try
        {
            // delete associated categories
            context.CategoryContents.RemoveRange(
               context.CategoryContents.Where(x => x.ContentId == entity.Id && x.Type == CategoryEnum.Types.Blogs));

            // delete associated cultures
            context.BlogData.RemoveRange(
               context.BlogData.Where(x => x.BlogId == entity.Id));

            context.Blogs.RemoveRange(
                context.Blogs.Where(x => x.Id == entity.Id));

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error here
        }
    }

    #endregion

    #region Content Management

    /// <summary>
    /// Adds localized content for a blog post
    /// </summary>
    private static async Task<BlogData?> AddContent(
        ApplicationDBContext context,
        int blogId,
        BlogData? entity)
    {
        if (entity == null) return null;

        try
        {
            var newContent = new BlogData
            {
                BlogId = blogId,
                Title = entity.Title,
                ShortDescription = entity.ShortDescription,
                Culture = entity.Culture,
                Description = entity.Description
            };

            context.Entry(newContent).State = EntityState.Added;
            await context.SaveChangesAsync();
            return newContent;
        }
        catch (Exception ex)
        {
            // Log error here
            return null;
        }
    }

    /// <summary>
    /// Updates localized content for a blog post
    /// </summary>
    private static async Task UpdateBlogData(
        ApplicationDBContext context,
        BlogData entity)
    {
        if (entity.Id <= 0) return;

        try
        {
            var existingContent = await context.BlogData
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (existingContent != null)
            {
                existingContent.Title = entity.Title;
                existingContent.ShortDescription = entity.ShortDescription;
                existingContent.Description = entity.Description;

                context.Entry(existingContent).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error here
        }
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Retrieves a single blog post by ID
    /// </summary>
    public static async Task<Blogs?> GetRecord(
        ApplicationDBContext context,
        int blogId)
    {
        return await context.Blogs
            .FirstOrDefaultAsync(p => p.Id == blogId);
    }

    /// <summary>
    /// Loads blog posts based on query parameters with optional caching
    /// </summary>
    public static async Task<List<Blogs>?> LoadItems(
        ApplicationDBContext context,
        BlogQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await FetchItems(context, entity);
        }

        var cacheKey = GenerateCacheKey("lng_blog", entity);
        var cachedData = SiteConfigs.Cache?.Get<List<Blogs>>(cacheKey);

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
    /// Counts blog posts matching query parameters with optional caching
    /// </summary>
    public static async Task<int> CountItems(
        ApplicationDBContext context,
        BlogQueryEntity entity)
    {
        if (!entity.IsCache || ApplicationSettings.Caching.Duration == 0)
        {
            return await Count(context, entity);
        }

        var cacheKey = GenerateCacheKey("cnt_blog", entity);

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
    /// Generates a unique slug for a blog post
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
        var existingCount = await Count(context, new BlogQueryEntity
        {
            Slug = slug,
            AdvanceFilter = false
        });

        if (existingCount == 0) return slug;

        // Handle duplicate slugs
        var totalOccurrences = await Count(context, new BlogQueryEntity
        {
            SlugStartedWith = slug,
            AdvanceFilter = false
        });

        var newSlug = $"{slug}-{totalOccurrences + 1}";

        // Verify new slug is unique
        existingCount = await Count(context, new BlogQueryEntity
        {
            Slug = newSlug,
            AdvanceFilter = false
        });

        return existingCount > 0
            ? $"{newSlug}-{DateTime.Now.Ticks.ToString().Substring(0, 10)}"
            : newSlug;
    }

    /// <summary>
    /// Processes batch actions on multiple blog posts
    /// </summary>
    public static async Task ProcessApiActions(
        ApplicationDBContext context,
        List<Blogs> list)
    {
        foreach (var item in list)
        {
            if (item.Id <= 0) continue;

            switch (item.ActionStatus?.ToLower())
            {
                case "approve":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Enabled, "IsApproved");
                    break;

                case "enable":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Enabled, "IsEnabled");
                    break;

                case "disable":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Disabled, "IsEnabled");
                    break;

                case "featured":
                    await UpdateField(context, item.Id, (byte)Types.FeaturedTypes.Featured, "IsFeatured");
                    break;

                case "normal":
                    await UpdateField(context, item.Id, (byte)Types.FeaturedTypes.Basic, "IsFeatured");
                    break;

                case "delete":
                    if (item.IsArchive == Types.ActionTypes.Enabled)
                    {
                        // Permanently Delete Record
                        await Delete(context, item);
                    }
                    else
                    {
                        // Place in Archive
                        await UpdateField(context, item.Id, (byte)Types.ActionTypes.Enabled, "IsArchive");
                    }
                    break;

                case "permament_delete":
                    await Delete(context, item);
                    break;

                case "restore":
                    await UpdateField(context, item.Id, (byte)Types.ActionTypes.Enabled, "IsEnabled");
                    break;
            }
        }
    }

    #endregion

    #region Private Helpers

    private static async Task<List<Blogs>> FetchItems(
        ApplicationDBContext context,
        BlogQueryEntity entity)
    {
        var query = ProcessConditions(PrepareQuery(context, entity), entity);
        return await LoadList(query, entity);
    }

    private static async Task<int> Count(
        ApplicationDBContext context,
        BlogQueryEntity entity)
    {
        return await PrepareQuery(context, entity).CountAsync();
    }

    /// <summary>
    /// Generate List (Processed Dynamic Queries)
    /// </summary>
    private static Task<List<Blogs>> LoadList(IQueryable<BlogQuery> query, BlogQueryEntity entity)
    {
        return query.Select(prepareList(entity)).ToListAsync();
    }


    /// <summary>
    /// Core function for preparing List
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<BlogQuery, Blogs>> prepareList(BlogQueryEntity entity)
    {
        return p => prepareListItem(p, entity);
    }


    /// <summary>
    /// Core function for preparing each list item
    /// </summary>

    private static Blogs prepareListItem(BlogQuery p, BlogQueryEntity entity)
    {
        var record = new Blogs()
        {
            Id = p.Blog.Id,
            Term = p.Blog.Term,
            Tags = p.Blog.Tags,
            CreatedAt = p.Blog.CreatedAt,
            IsEnabled = p.Blog.IsEnabled,
            IsApproved = p.Blog.IsApproved,
            Views = p.Blog.Views,
            Comments = p.Blog.Comments,
            IsFeatured = p.Blog.IsFeatured,
            IsArchive = p.Blog.IsArchive,
            Cover = p.Blog.Cover,
            UpdatedAt = p.Blog.UpdatedAt
        };

        // this will return default thumb or cover url if user not updated avatar, cover yet
        //record.Cover = UploadMedia.GetImageUrl(p.Blog.Cover, BlogSettings.DefaultThumbnail);

        // fields will be available if 
        // list view or profile view
        // id > 0
        // slug which is equivalent with id != ""
        if (entity.ColumnOptions == Types.FetchColumnOptions.List
            || entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || entity.Id > 0
            || !string.IsNullOrEmpty(entity.Slug))
        {
            // ...
        }

        record.Url = Urls.PreviewUrl(new UrlEntity()
        {
            Id = p.Blog.Id,
            Title = p.Blog.Term,
            Directory = BlogUrls.GetDirectoryUrl()
        });

        // profile view
        if (entity.ColumnOptions == Types.FetchColumnOptions.Profile
            || entity.Id > 0
            || !string.IsNullOrEmpty(entity.Slug))
        {
            // record.description = p.blog.description;
            record.ArchiveAt = p.Blog.ArchiveAt;

        }

        if (p.Data != null)
        {
            record.BlogData = new BlogData()
            {
                Id = p.Data.Id,
                Title = p.Data.Title,
                ShortDescription = p.Data.ShortDescription,
                Description = p.Data.Description,
                MetaDescription = p.Data.MetaDescription,
                Culture = p.Data.Culture
            };


        }

        // Attach company information if not null
        if (p.User != null)
        {
            record.Author = new ApplicationUser()
            {
                Id = p.User.Id,
                FirstName = p.User.FirstName,
                Avatar = p.User.Avatar
            };


        }



        return record;
    }


    /// <summary>
    /// Core function for handling pagination or fetching specific no of records
    /// </summary>
    public static IQueryable<BlogQuery> ProcessConditions(IQueryable<BlogQuery> collectionQuery, BlogQueryEntity query)
    {
        if (!string.IsNullOrEmpty(query.Order))
            collectionQuery = (IQueryable<BlogQuery>)collectionQuery.Sort(query.Order);

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


    private static IQueryable<BlogQuery> PrepareQuery(
        ApplicationDBContext context,
        BlogQueryEntity entity)
    {
        if (!string.IsNullOrEmpty(entity.CategoryName) ||
            entity.CategoryId > 0 ||
            entity.CategoryIds != null)
        {
            return context.Blogs
                .Join(context.BlogData,
                    blog => blog.Id,
                    data => data.BlogId,
                    (blog, data) => new { blog, data })
                .Join(context.AspNetUsers,
                    blog => blog.blog.UserId,
                    user => user.Id,
                    (blog, user) => new { blog, user })
                .Join(context.CategoryContents,
                    blog => blog.blog.blog.Id,
                    categoryContent => categoryContent.ContentId,
                    (blog, categoryContent) => new { blog, categoryContent })
                .Join(context.Categories,
                    categoryContent => categoryContent.categoryContent.CategoryId,
                    category => category.Id,
                    (categoryContent, category) => new { categoryContent, category })
                .Join(context.CategoryData,
                    categoryContent => categoryContent.category.Id,
                    categoryData => categoryData.CategoryId,
                    (categoryContent, categoryData) => new BlogQuery
                    {
                        Blog = categoryContent.categoryContent.blog.blog.blog,
                        Data = categoryContent.categoryContent.blog.blog.data,
                        User = categoryContent.categoryContent.blog.user,
                        CategoryContent = categoryContent.categoryContent.categoryContent,
                        CategoryData = categoryData,
                        Category = categoryContent.category
                    })
                .Where(ReturnWhereClause(entity));
        }

        return context.Blogs
            .Join(context.BlogData,
                blog => blog.Id,
                data => data.BlogId,
                (blog, data) => new { blog, data })
            .Join(context.AspNetUsers,
                blog => blog.blog.UserId,
                user => user.Id,
                (blog, user) => new BlogQuery
                {
                    Blog = blog.blog,
                    Data = blog.data,
                    User = user
                })
            .Where(ReturnWhereClause(entity));
    }

    public static Expression<Func<BlogQuery, bool>> ReturnWhereClause(
        BlogQueryEntity entity)
    {
        var predicate = PredicateBuilder.New<BlogQuery>(true);
        if (string.IsNullOrEmpty(entity.Culture))
            entity.Culture = "en"; // default

        if (entity.Id > 0)
        {
            predicate = predicate.And(p => p.Blog != null && p.Blog.Id == entity.Id);
        }
        else if (!string.IsNullOrEmpty(entity.Slug))
        {
            predicate = predicate.And(p => p.Blog != null && p.Blog.Term == entity.Slug);
        }
        else if (!string.IsNullOrEmpty(entity.SlugStartedWith))
        {
            predicate = predicate.And(p => p.Blog != null &&
                !string.IsNullOrEmpty(p.Blog.Term) &&
                p.Blog.Term.StartsWith(entity.SlugStartedWith));
        }

        if (entity.AdvanceFilter)
        {
            if (!string.IsNullOrEmpty(entity.Culture))
            {
                predicate = predicate.And(p => p.Data != null && p.Data.Culture == entity.Culture);
            }

            // if groupby not none (report query)
            if (entity.GroupBy != BlogEnum.ChartGroupBy.None)
            {
                switch (entity.GroupBy)
                {
                    case BlogEnum.ChartGroupBy.Categories:
                        predicate = predicate.And(p => p.CategoryContent != null && p.CategoryContent.Type == CategoryEnum.Types.Blogs);
                        break;
                }
            }

            // if category filter involve
            if (!string.IsNullOrEmpty(entity.CategoryName) || entity.CategoryId > 0 || entity.CategoryIds != null)
            {
                // category content table can save multiple type of contents, we need only filter with company
                predicate = predicate.And(p => p.CategoryContent != null && p.CategoryContent.Type == CategoryEnum.Types.Blogs);
                // filter category by culture
                predicate = predicate.And(p => p.CategoryData != null && p.CategoryData.Culture == entity.Culture);
                if (!string.IsNullOrEmpty(entity.CategoryName))
                {
                    predicate = predicate.And(p => p.Category != null && p.CategoryData != null
                    && (p.CategoryData.Title == entity.CategoryName // map with directory category name e.g Entertainment
                    || p.Category.Term == entity.CategoryName // map with slug or term e.g /entertainment
                    || p.Category.SubTerm == entity.CategoryName // map with complete slug or term e.g /company/graphics/rtx/rtx-3090
                    ));
                }
                else if (entity.CategoryId > 0)
                {
                    predicate = predicate.And(p => p.Category != null && p.Category.Id == entity.CategoryId);
                }
                else if (entity.CategoryIds != null)
                {
                    foreach (var categoryid in entity.CategoryIds)
                    {
                        predicate = predicate.And(p => p.Category != null && p.Category.Id == categoryid);
                    }
                }
            }

            // Filter records by tag or label
            if (!string.IsNullOrEmpty(entity.Tags))
            {
                entity.Tags = UtilityHelper.ReplaceHyphensWithSpaces(entity.Tags);
                predicate = predicate.And(p => p.Blog != null && !string.IsNullOrEmpty(p.Blog.Tags) && p.Blog.Tags.ToLower().Contains(entity.Tags));
            }
               

            // set condition to load only public company records
            if (entity.IsPublic)
            {
                predicate = predicate.And(p => p.Blog != null
                && p.Blog.IsEnabled == Types.ActionTypes.Enabled
                   && p.Blog.IsApproved == Types.ActionTypes.Enabled
                   && p.Blog.IsArchive == Types.ActionTypes.Disabled
                );
            }
            else
            {
                // manual control on conditions for fetching company records

                if (entity.IsEnabled != Types.ActionTypes.All)
                    predicate = predicate.And(p => p.Blog != null && p.Blog.IsEnabled == entity.IsEnabled);

                if (entity.IsApproved != Types.ActionTypes.All)
                    predicate = predicate.And(p => p.Blog != null && p.Blog.IsApproved == entity.IsApproved);

                if (entity.IsArchive != Types.ActionTypes.All)
                    predicate = predicate.And(p => p.Blog != null && p.Blog.IsArchive == entity.IsArchive);
            }

            if (entity.IsFeatured != Types.FeaturedTypes.All)
                predicate = predicate.And(p => p.Blog != null && p.Blog.IsFeatured == entity.IsFeatured);

            if (!string.IsNullOrEmpty(entity.UserId))
                predicate = predicate.And(p => p.Blog != null && p.Blog.UserId == entity.UserId);

            if (!string.IsNullOrEmpty(entity.UserSlug))
                predicate = predicate.And(p => p.User != null && p.User.Slug == entity.UserSlug);


            // add more and more based on requirements...

            var _date = UtilityHelper.TimeZoneOffsetDateTime();

            // Filter records based on start and end dates

            if (entity.StartDate != null)
                predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= entity.StartDate.Value.Date);

            if (entity.EndDate != null)
                predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date <= entity.EndDate.Value.Date);


            // Archive Expiry Filters
            if (entity.ArchiveExpiry != Types.ArchiveExpiryOptions.All)
            {
                switch (entity.ArchiveExpiry)
                {
                    case Types.ArchiveExpiryOptions.Expired:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.ArchiveAt > _date && p.Blog.ArchiveAt <= _date.AddDays(1));
                        break;

                    case Types.ArchiveExpiryOptions.ExpireToday:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.ArchiveAt > _date.AddDays(-1));
                        break;

                    case Types.ArchiveExpiryOptions.Expire_in_5Days:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.ArchiveAt > _date.AddDays(-6));
                        break;

                }
            }

            // Add filter conditions for reporting and order management

            if (entity.DateFilter != Types.DateFilter.All)
            {
                // start of month date

                var start_date = new DateTime(_date.Year, _date.Month, 1);
                switch (entity.DateFilter)
                {
                    case Types.DateFilter.Today:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt >= _date.AddDays(-1));
                        break;
                    case Types.DateFilter.ThisWeek:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt >= _date.AddDays(-7));
                        break;
                    case Types.DateFilter.ThisMonth:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt >= start_date.Date);
                        break;
                    case Types.DateFilter.PrevMonth:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= start_date.AddMonths(-1)
                           && p.Blog.CreatedAt.Date < start_date.Date);
                        break;

                    case Types.DateFilter.CurrentPrevMonth:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= start_date.AddMonths(-1));
                        break;

                    case Types.DateFilter.PrevThreeMonths:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= start_date.AddMonths(-2));
                        break;

                    case Types.DateFilter.PrevSixMonths:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= start_date.AddMonths(-5));
                        break;

                    case Types.DateFilter.ThisYear:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Year >= start_date.Year);
                        break;

                    case Types.DateFilter.ThisHour:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt >= _date.AddHours(-1));
                        break;

                    case Types.DateFilter.LastSixHour:
                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt >= _date.AddHours(-6));
                        break;

                    case Types.DateFilter.PrevYear:
                        DateTime firsday = new DateTime(_date.Year, 1, 1);
                        DateTime lastday = new DateTime(_date.Year, 12, 31);

                        predicate = predicate.And(p => p.Blog != null && p.Blog.CreatedAt.Date >= firsday.AddYears(-1)
                        && p.Blog.CreatedAt.Date < lastday.AddYears(-1));

                        break;

                }
            }

            // broad search
            if (!string.IsNullOrEmpty(entity.Term))
            {
                predicate = predicate.And(p => p.Blog != null
                   && (p.Data.Title.Contains(entity.Term)
                   || p.Data.ShortDescription.Contains(entity.Term)
                   || p.Data.Description.Contains(entity.Term)
                   || p.Blog.Id.ToString() == entity.Term
                   ));
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

        var item = await context.Blogs
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

    private static string GenerateCacheKey(string prefix, BlogQueryEntity entity)
    {
        var builder = new StringBuilder()
            .Append(prefix)
            .Append('_')
            .Append(entity.PageNumber)
            .Append(entity.PageSize)
            .Append(entity.IsFeatured)
            .Append(entity.CategoryId);

        if (!string.IsNullOrEmpty(entity.Order))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(
                entity.Order.ToLower()));
        }

        if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            builder.Append(UtilityHelper.ReplaceSpaceWithHyphen(
                entity.CategoryName.ToLower()));
        }

        return builder.ToString();
    }

    #endregion
}

/// <summary>
/// Represents a joined result of a blog post with related data
/// </summary>
[JsonSerializable(typeof(BlogQuery))]
public class BlogQuery
{
    [JsonPropertyName("blog")]
    public Blogs? Blog { get; set; }

    [JsonPropertyName("data")]
    public BlogData? Data { get; set; }

    [JsonPropertyName("user")]
    public ApplicationUser? User { get; set; }

    [JsonPropertyName("category_content")]
    public CategoryContents? CategoryContent { get; set; }

    [JsonPropertyName("category_data")]
    public CategoryData? CategoryData { get; set; }

    [JsonPropertyName("category")]
    public Categories? Category { get; set; }
}