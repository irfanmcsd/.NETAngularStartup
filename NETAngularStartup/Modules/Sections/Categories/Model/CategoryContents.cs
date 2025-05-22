using DevCodeArchitect.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents a mapping between categories and their associated content items.
/// This class establishes relationships between categories and various types of content.
/// </summary>
public class CategoryContents
{
    /// <summary>
    /// Primary key identifier for the category-content relationship
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
    /// Foreign key reference to the associated content item
    /// JSON Property: "contentid" (integer)
    /// </summary>
    [JsonPropertyName("contentid")]
    public int ContentId { get; set; }

    /// <summary>
    /// Content type identifier (e.g., 1=properties, 2=companies, 3=blogs)
    /// JSON Property: "type" (byte)
    /// </summary>
    [JsonPropertyName("type")]
    public CategoryEnum.Types Type { get; set; }

    // Navigation properties (not mapped to database)

    /// <summary>
    /// Navigation property to the associated category (not mapped to database)
    /// JSON Property: "category" (object, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("category")]
    public Categories? Category { get; set; }

    /// <summary>
    /// Navigation property to localized category data (not mapped to database)
    /// JSON Property: "data" (object, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("data")]
    public CategoryData? Data { get; set; }
}