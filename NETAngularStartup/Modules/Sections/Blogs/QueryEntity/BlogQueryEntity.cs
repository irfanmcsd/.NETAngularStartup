using DevCodeArchitect.Entity;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Specialized query entity for blog posts with additional filtering and grouping options.
/// Extends the base ContentEntity with blog-specific query parameters.
/// </summary>
[JsonSerializable(typeof(BlogQueryEntity))]
public class BlogQueryEntity : ContentEntity
{
    /// <summary>
    /// User slug identifier for filtering blog posts by author
    /// JSON Property: "user_slug" (string, nullable)
    /// Example: "john-doe"
    /// </summary>
    [JsonPropertyName("user_slug")]
    public string? UserSlug { get; set; }

    /// <summary>
    /// Specifies how blog post data should be grouped in analytical queries
    /// JSON Property: "groupby" (string)
    /// Default: BlogEnum.ChartGroupBy.None
    /// </summary>
    [JsonPropertyName("groupby")]
    public BlogEnum.ChartGroupBy GroupBy { get; set; } = BlogEnum.ChartGroupBy.None;

    /// <summary>
    /// Filter for featured blog posts
    /// JSON Property: "isfeatured" (string)
    /// Default: Types.FeaturedTypes.All (no filtering)
    /// </summary>
    [JsonPropertyName("isfeatured")]
    public Types.FeaturedTypes IsFeatured { get; set; } = Types.FeaturedTypes.All;


}