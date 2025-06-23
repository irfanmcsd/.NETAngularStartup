using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents localized content data for blog posts, enabling multi-language support.
/// Contains culture-specific versions of blog content and metadata.
/// </summary>
public class BlogData
{
    /// <summary>
    /// Primary key identifier for the blog data record
    /// JSON Property: "id" (integer)
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key reference to the associated blog post
    /// JSON Property: "blogid" (integer)
    /// </summary>
    [JsonPropertyName("blogid")]
    public int BlogId { get; set; }

    /// <summary>
    /// Culture code for this localized content (e.g., "en-US", "fr-FR")
    /// JSON Property: "culture" (string)
    /// Default: Empty string
    /// </summary>
    [JsonPropertyName("culture")]
    public string Culture { get; set; } = string.Empty;

    /// <summary>
    /// Localized title of the blog post
    /// JSON Property: "title" (string)
    /// Default: Empty string
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Short description/teaser for the blog post (optional)
    /// JSON Property: "short_description" (string, nullable)
    /// </summary>
    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; set; }

    /// <summary>
    /// Full content of the blog post in HTML or Markdown format
    /// JSON Property: "description" (string)
    /// Default: Empty string
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Secondary localized title or tagline (optional)
    /// JSON Property: "sub_title" (string, nullable)
    /// </summary>
    [JsonPropertyName("metaDescription")]
    public string? MetaDescription { get; set; }

    /// <summary>
    /// Navigation property to the associated category (not mapped to database)
    /// JSON Property: "category" (object, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("category")]
    public Categories? Category { get; set; }
}