using DevCodeArchitect.Entity;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides utility methods for constructing category URLs with support for hierarchical category structures.
/// Ensures consistent URL formatting across the application.
/// </summary>
public static class CategoryUrls
{
    /// <summary>
    /// Constructs a basic category URL in the format: {path}/{categoryName}
    /// </summary>
    /// <param name="path">The base path/route (e.g., "categories")</param>
    /// <param name="categoryName">The category name/slug (nullable)</param>
    /// <returns>
    /// Formatted URL string. Returns empty string if categoryName is null or whitespace.
    /// </returns>
    /// <example>
    /// GetCategoryUrl("products", "electronics") → "products/electronics"
    /// </example>
    public static string GetCategoryUrl(string path, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        return $"{path.Trim('/')}/{categoryName.Trim('/')}";
    }

    /// <summary>
    /// Constructs a two-level category URL in the format: {path}/{child1}/{categoryName}
    /// </summary>
    /// <param name="path">The base path/route</param>
    /// <param name="child1">First-level child category (nullable)</param>
    /// <param name="categoryName">The category name/slug (nullable)</param>
    /// <returns>
    /// Formatted URL string. Returns empty string if any required segment is null or whitespace.
    /// </returns>
    /// <example>
    /// GetCategoryUrl("products", "computers", "laptops") → "products/computers/laptops"
    /// </example>
    public static string GetCategoryUrl(string path, string? child1, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(child1) || string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        return $"{path.Trim('/')}/{child1.Trim('/')}/{categoryName.Trim('/')}";
    }

    /// <summary>
    /// Constructs a three-level category URL in the format: {path}/{child2}/{child1}/{categoryName}
    /// </summary>
    /// <param name="path">The base path/route</param>
    /// <param name="child2">Second-level child category (nullable)</param>
    /// <param name="child1">First-level child category (nullable)</param>
    /// <param name="categoryName">The category name/slug (nullable)</param>
    /// <returns>
    /// Formatted URL string. Returns empty string if any required segment is null or whitespace.
    /// </returns>
    /// <example>
    /// GetCategoryUrl("products", "electronics", "computers", "gaming-laptops") → 
    /// "products/electronics/computers/gaming-laptops"
    /// </example>
    public static string GetCategoryUrl(string path, string? child2, string? child1, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(child2) ||
            string.IsNullOrWhiteSpace(child1) ||
            string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        return $"{path.Trim('/')}/{child2.Trim('/')}/{child1.Trim('/')}/{categoryName.Trim('/')}";
    }

    /// <summary>
    /// Constructs a four-level category URL in the format: {path}/{child3}/{child2}/{child1}/{categoryName}
    /// </summary>
    /// <param name="path">The base path/route</param>
    /// <param name="child3">Third-level child category (nullable)</param>
    /// <param name="child2">Second-level child category (nullable)</param>
    /// <param name="child1">First-level child category (nullable)</param>
    /// <param name="categoryName">The category name/slug (nullable)</param>
    /// <returns>
    /// Formatted URL string. Returns empty string if any required segment is null or whitespace.
    /// </returns>
    public static string GetCategoryUrl(string path, string? child3, string? child2, string? child1, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(child3) ||
            string.IsNullOrWhiteSpace(child2) ||
            string.IsNullOrWhiteSpace(child1) ||
            string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        return $"{path.Trim('/')}/{child3.Trim('/')}/{child2.Trim('/')}/{child1.Trim('/')}/{categoryName.Trim('/')}";
    }

    /// <summary>
    /// Constructs a five-level category URL in the format: {path}/{child4}/{child3}/{child2}/{child1}/{categoryName}
    /// </summary>
    /// <param name="path">The base path/route</param>
    /// <param name="child4">Fourth-level child category (nullable)</param>
    /// <param name="child3">Third-level child category (nullable)</param>
    /// <param name="child2">Second-level child category (nullable)</param>
    /// <param name="child1">First-level child category (nullable)</param>
    /// <param name="categoryName">The category name/slug (nullable)</param>
    /// <returns>
    /// Formatted URL string. Returns empty string if any required segment is null or whitespace.
    /// </returns>
    public static string GetCategoryUrl(string path, string? child4, string? child3, string? child2, string? child1, string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(child4) ||
            string.IsNullOrWhiteSpace(child3) ||
            string.IsNullOrWhiteSpace(child2) ||
            string.IsNullOrWhiteSpace(child1) ||
            string.IsNullOrWhiteSpace(categoryName))
            return string.Empty;

        return $"{path.Trim('/')}/{child4.Trim('/')}/{child3.Trim('/')}/{child2.Trim('/')}/{child1.Trim('/')}/{categoryName.Trim('/')}";
    }
}