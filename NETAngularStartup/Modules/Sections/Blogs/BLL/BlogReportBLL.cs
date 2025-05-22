using DevCodeArchitect.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Business Logic Layer for generating analytical reports on blog data.
/// Supports grouping by various time periods and categories.
/// </summary>
public static class BlogReportBLL
{
    /// <summary>
    /// Generates a blog report based on the specified grouping criteria
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Query parameters and grouping specification</param>
    /// <returns>
    /// Report data grouped according to the specified criteria.
    /// Returns different report formats based on grouping type.
    /// </returns>
    public static async Task<object> Generate(ApplicationDBContext context, BlogQueryEntity entity)
    {
        return entity.GroupBy switch
        {
            BlogEnum.ChartGroupBy.Hour => await GroupByHour(context, entity),
            BlogEnum.ChartGroupBy.Day => await GroupByDay(context, entity),
            BlogEnum.ChartGroupBy.Year => await GroupByYear(context, entity),
            BlogEnum.ChartGroupBy.Categories => await GroupByCategories(context, entity),
            _ => await GroupByMonth(context, entity) // Default to month grouping
        };
    }

    /// <summary>
    /// Groups blog data by associated categories
    /// </summary>
    /// <returns>
    /// List of Categories objects with counts of blog posts in each category
    /// </returns>
    public static async Task<List<CategoryReport>> GroupByCategories(ApplicationDBContext context, BlogQueryEntity entity)
    {
        return await context.Blogs
            .Join(context.CategoryContents,
                blog => blog.Id,
                blogCategory => blogCategory.ContentId,
                (blog, blogCategory) => new { blog, blogCategory })
            .Join(context.CategoryData,
                joined => joined.blogCategory.CategoryId,
                categoryData => categoryData.CategoryId,
                (joined, categoryData) => new { joined, categoryData })
            .Join(context.Categories,
                joined => joined.categoryData.CategoryId,
                category => category.Id,
                (joined, category) => new BlogQuery
                {
                    Blog = joined.joined.blog,
                    CategoryContent = joined.joined.blogCategory,
                    CategoryData = joined.categoryData,
                    Category = category
                })
            .Where(BlogsBLL.ReturnWhereClause(entity))
            .GroupBy(g => new
            {
                g.Category.Id,
                g.Category.Term,
                g.CategoryData.Title
            })
            .Select(g => new CategoryReport
            {
                CategoryId = g.Key.Id,
                Term = g.Key.Term,
                Label = g.Key.Title,
                PostCount = g.Count()
            })
            .OrderBy(r => r.Label)
            .ToListAsync();
    }

    /// <summary>
    /// Groups blog data by hour of creation
    /// </summary>
    /// <returns>
    /// List of ReportEntity objects with counts of blog posts per hour
    /// </returns>
    public static async Task<List<TimeReport>> GroupByHour(ApplicationDBContext context, BlogQueryEntity entity)
    {
        return await context.Blogs
            .Select(m => new BlogQuery
            {
                Blog = m
            })
            .Where(BlogsBLL.ReturnWhereClause(entity))
            .GroupBy(b => b.Blog.CreatedAt.Hour)
            .Select(g => new TimeReport
            {
                TimeValue = g.Key,
                Label = $"{g.Key}:00",
                Count = g.Count()
            })
            .OrderBy(g => g.TimeValue)
            .ToListAsync();
    }

    /// <summary>
    /// Groups blog data by day of creation
    /// </summary>
    /// <returns>
    /// List of ReportEntity objects with counts of blog posts per day
    /// </returns>
    public static async Task<List<TimeReport>> GroupByDay(ApplicationDBContext context, BlogQueryEntity entity)
    {
        return await context.Blogs
           .Select(m => new BlogQuery
           {
               Blog = m
           })
           .Where(BlogsBLL.ReturnWhereClause(entity))
           .GroupBy(b => b.Blog.CreatedAt.Day)
           .Select(g => new TimeReport
           {
               TimeValue = g.Key,
               Label = g.Key.ToString(),
               Count = g.Count()
           })
           .OrderBy(g => g.TimeValue)
           .ToListAsync();
    }

    /// <summary>
    /// Groups blog data by month of creation
    /// </summary>
    /// <returns>
    /// List of ReportEntity objects with counts of blog posts per month
    /// </returns>
    public static async Task<List<MonthlyReport>> GroupByMonth(ApplicationDBContext context, BlogQueryEntity entity)
    {
        var reportData = await context.Blogs
            .Select(m => new BlogQuery
            {
                Blog = m
            })
           .Where(BlogsBLL.ReturnWhereClause(entity))
           .GroupBy(b => new { b.Blog.CreatedAt.Month, b.Blog.CreatedAt.Year })
           .Select(g => new MonthlyReport
           {
               Month = g.Key.Month,
               Year = g.Key.Year,
               Count = g.Count()
           })
           .OrderBy(r => r.Year)
           .ThenBy(r => r.Month)
           .ToListAsync();

        foreach (var item in reportData)
        {
            item.Label = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month)} {item.Year}";
        }

        return reportData;

    }

    /// <summary>
    /// Groups blog data by year of creation
    /// </summary>
    /// <returns>
    /// List of ReportEntity objects with counts of blog posts per year
    /// </returns>
    public static async Task<List<YearlyReport>> GroupByYear(ApplicationDBContext context, BlogQueryEntity entity)
    {
        return await context.Blogs
            .Select(m => new BlogQuery
            {
                Blog = m
            })
           .Where(BlogsBLL.ReturnWhereClause(entity))
           .GroupBy(b => b.Blog.CreatedAt.Year)
           .Select(g => new YearlyReport
           {
               Year = g.Key,
               Label = g.Key.ToString(),
               Count = g.Count()
           })
           .OrderBy(r => r.Year)
           .ToListAsync();

      
    }
}

/// <summary>
/// Report DTO for category-based grouping
/// </summary>
public class CategoryReport
{
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    [JsonPropertyName("term")]
    public string Term { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("post_count")]
    public int PostCount { get; set; }
}

/// <summary>
/// Base report DTO for time-based groupings
/// </summary>
public class TimeReport
{
    [JsonPropertyName("time_value")]
    public int TimeValue { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>
/// Report DTO for monthly grouping
/// </summary>
public class MonthlyReport : TimeReport
{
    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
}

/// <summary>
/// Report DTO for yearly grouping
/// </summary>
public class YearlyReport : TimeReport
{
    [JsonPropertyName("year")]
    public new int Year { get; set; }
}