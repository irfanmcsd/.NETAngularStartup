using DevCodeArchitect.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents a category entity in the database with hierarchical capabilities.
/// This class is used to organize content into structured categories with parent-child relationships.
/// </summary>
public class Categories
{
    /// <summary>
    /// Primary key identifier for the category
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The main term/slug that identifies the category (e.g., "entertainment")
    /// </summary>
    [JsonPropertyName("term")]
    public string? Term { get; set; }

    /// <summary>
    /// Extended slug path for nested categories (e.g., "/computers/graphic-cards/rtx-4090")
    /// </summary>
    [JsonPropertyName("sub_term")]
    public string? SubTerm { get; set; }

    /// <summary>
    /// ID of the parent category (0 indicates a root category)
    /// </summary>
    [JsonPropertyName("parentid")]
    public int ParentId { get; set; }

    /// <summary>
    /// Type identifier for grouping categories (e.g., 1=properties, 2=blogs, 3=companies)
    /// </summary>
    [JsonPropertyName("type")]
    public CategoryEnum.Types Type { get; set; }

    /// <summary>
    /// Priority value used for ordering categories (lower numbers have higher priority)
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>
    /// Flag indicating whether the category is enabled/active (0=disabled, 1=enabled)
    /// </summary>
    [JsonPropertyName("isenabled")]
    public Types.ActionTypes IsEnabled { get; set; }

    /// <summary>
    /// Flag indicating whether the category is enabled/active (0=disabled, 1=enabled)
    /// </summary>
    [JsonPropertyName("createAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Flag indicating whether the category is enabled/active (0=disabled, 1=enabled)
    /// </summary>
    [JsonPropertyName("updateAt")]
    public DateTime? UpdatedAt { get; set; }


    /// <summary>
    /// Flag indicating if the category is featured (0=normal, 1=featured)
    /// </summary>
    [JsonPropertyName("isfeatured")]
    public Types.FeaturedTypes IsFeatured { get; set; }

    /// <summary>
    /// URL or path to the category's avatar/image
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// Count of records associated with this category
    /// </summary>
    [JsonPropertyName("records")]
    public int Records { get; set; }

    // Navigation properties and computed fields (not mapped to database)

    /// <summary>
    /// Additional category data (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("category_data")]
    public CategoryData? CategoryData { get; set; }

    /// <summary>
    /// List of localized category data for different cultures (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("culture_categories")]
    public List<CategoryData>? CultureCategories { get; set; }

    /// <summary>
    /// Status message for UI actions (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("actionstatus")]
    public string? ActionStatus { get; set; }

    /// <summary>
    /// Display label for the category (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Total count used for pagination (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Total count used for pagination (not mapped to database)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("children")]
    public List<Categories>? Children { get; set; }

    
}