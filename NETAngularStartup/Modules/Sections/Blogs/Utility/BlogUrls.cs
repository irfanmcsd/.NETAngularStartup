using DevCodeArchitect.Entity;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides URL generation utilities for blog-related routes in the application
/// </summary>
/// <remarks>
/// Centralizes URL construction logic for blog posts, categories, tags, and featured content.
/// Ensures consistent URL patterns throughout the application.
/// </remarks>
public static class BlogUrls
{
    #region Base URLs

    /// <summary>
    /// Gets the base directory URL for blog content
    /// </summary>
    /// <returns>The relative URL path for blog directory ("post")</returns>
    public static string GetDirectoryUrl() => "post";

    /// <summary>
    /// Gets the base URL for the blog section
    /// </summary>
    /// <returns>The relative URL path for blogs ("/blogs")</returns>
    public static string GetUrl(string culture) => $"/blogs/{culture}";

    #endregion

    #region Tag/Label URLs

    /// <summary>
    /// Gets the base URL for blog tags
    /// </summary>
    /// <returns>The relative URL path for blog tags ("/blogs/tags")</returns>
    public static string GetLabelUrl(string culture) => $"/blogs/tags/{culture}";

    /// <summary>
    /// Generates a URL for a specific blog tag
    /// </summary>
    /// <param name="label">The tag name (URL-encoded)</param>
    /// <returns>
    /// Formatted URL path: "/blogs/tag/{label}"
    /// </returns>
    public static string GetLabelUrl(string? culture, string? label) => $"/blogs/tag/{culture}/{label}";

    #endregion

    #region Featured Content URLs

    /// <summary>
    /// Generates a URL for featured blog content
    /// </summary>
    /// <param name="featured">The featured content type</param>
    /// <returns>
    /// Formatted URL path: "/blogs/type/{featuredType}"
    /// </returns>
    public static string GetFeaturedUrl(string culture, Types.FeaturedTypes? featured)
        => $"/blogs/type/{culture}/{Types.ParseFeaturedTypes(featured)}";

    #endregion

    #region Category URLs (Hierarchical Overloads)

    /// <summary>
    /// Generates a URL for a blog category
    /// </summary>
    /// <param name="categoryName">The category name</param>
    /// <returns>
    /// Formatted URL path: "/blogs/{categoryName}"
    /// </returns>
    public static string GetCategoryUrl(string culture, string? categoryName)
        => $"{GetUrl(culture)}/{categoryName}";

    /// <summary>
    /// Generates a URL for a blog category with one child level
    /// </summary>
    /// <param name="child1">First child category</param>
    /// <param name="categoryName">Parent category name</param>
    /// <returns>
    /// Formatted URL path: "/blogs/{child1}/{categoryName}"
    /// </returns>
    public static string GetCategoryUrl(string culture, string? child1, string? categoryName)
        => $"{GetUrl(culture)}/{child1}/{categoryName}";

    /// <summary>
    /// Generates a URL for a blog category with two child levels
    /// </summary>
    /// <param name="child2">Second child category</param>
    /// <param name="child1">First child category</param>
    /// <param name="categoryName">Parent category name</param>
    /// <returns>
    /// Formatted URL path: "/blogs/{child2}/{child1}/{categoryName}"
    /// </returns>
    public static string GetCategoryUrl(string culture, string? child2, string? child1, string? categoryName)
        => $"{GetUrl(culture)}/{child2}/{child1}/{categoryName}";

    /// <summary>
    /// Generates a URL for a blog category with three child levels
    /// </summary>
    /// <param name="child3">Third child category</param>
    /// <param name="child2">Second child category</param>
    /// <param name="child1">First child category</param>
    /// <param name="categoryName">Parent category name</param>
    /// <returns>
    /// Formatted URL path: "/blogs/{child3}/{child2}/{child1}/{categoryName}"
    /// </returns>
    public static string GetCategoryUrl(string culture, string? child3, string? child2, string? child1, string? categoryName)
        => $"{GetUrl(culture)}/{child3}/{child2}/{child1}/{categoryName}";

    /// <summary>
    /// Generates a URL for a blog category with four child levels
    /// </summary>
    /// <param name="child4">Fourth child category</param>
    /// <param name="child3">Third child category</param>
    /// <param name="child2">Second child category</param>
    /// <param name="child1">First child category</param>
    /// <param name="categoryName">Parent category name</param>
    /// <returns>
    /// Formatted URL path: "/blogs/{child4}/{child3}/{child2}/{child1}/{categoryName}"
    /// </returns>
    public static string GetCategoryUrl(string culture, string? child4, string? child3, string? child2, string? child1, string? categoryName)
        => $"{GetUrl(culture)}/{child4}/{child3}/{child2}/{child1}/{categoryName}";

    #endregion
}