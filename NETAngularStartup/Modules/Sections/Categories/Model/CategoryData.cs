using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents localized data for a category, containing culture-specific information
/// such as titles and descriptions. This class supports multilingual category content.
/// </summary>
public class CategoryData
{
    /// <summary>
    /// Primary key identifier for the category data record
    /// JSON Property: "id" (integer)
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key reference to the associated category
    /// JSON Property: "categoryid" (integer)
    /// </summary>
    [JsonPropertyName("categoryid")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Culture code for this localized content (e.g., "en-US", "fr-FR")
    /// JSON Property: "culture" (string)
    /// Default: Empty string
    /// </summary>
    [JsonPropertyName("culture")]
    public string Culture { get; set; } = string.Empty;

    /// <summary>
    /// Primary localized title for the category
    /// JSON Property: "title" (string)
    /// Default: Empty string
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Secondary localized title or tagline (optional)
    /// JSON Property: "sub_title" (string, nullable)
    /// </summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>
    /// Detailed description of the category in the specified culture
    /// JSON Property: "description" (string, nullable)
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property to the parent category (not mapped to database)
    /// JSON Property: "category" (object, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("category")]
    public Categories? Category { get; set; }
}