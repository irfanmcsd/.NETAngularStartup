using DevCodeArchitect.Entity;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevCodeArchitect.DBContext;

/// <summary>
/// Represents a blog post entity in the system with multi-language support,
/// content management features, and various status flags.
/// </summary>

public class Blogs
{
    #region Core Properties

    /// <summary>
    /// Primary key identifier for the blog post
    /// JSON Property: "id" (integer)
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// ID of the user who created the blog post
    /// JSON Property: "userid" (string)
    /// </summary>
    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// URL slug/identifier for the blog post
    /// JSON Property: "term" (string)
    /// </summary>
    [JsonPropertyName("term")]
    public string Term { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of tags for categorization
    /// JSON Property: "tags" (string, nullable)
    /// </summary>
    [JsonPropertyName("tags")]
    public string? Tags { get; set; } = string.Empty;

    /// <summary>
    /// URL or path to the blog post cover image
    /// JSON Property: "cover" (string, nullable)
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    #endregion

    #region Timestamps

    /// <summary>
    /// Date and time when the blog post was created
    /// JSON Property: "created_at" (string in ISO 8601 format)
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the blog post was last updated
    /// JSON Property: "updated_at" (string in ISO 8601 format, nullable)
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Date when the blog post was archived
    /// JSON Property: "archive_at" (string in ISO 8601 format, nullable)
    /// </summary>
    [JsonPropertyName("archive_at")]
    public DateTime? ArchiveAt { get; set; }

    #endregion

    #region Status Flags

    /// <summary>
    /// Indicates if the blog post is enabled (1) or blocked (0)
    /// JSON Property: "isenabled" (byte)
    /// </summary>
    [JsonPropertyName("isenabled")]
    public Types.ActionTypes IsEnabled { get; set; }

    /// <summary>
    /// Approval status: 0 = Pending, 1 = Approved
    /// JSON Property: "isapproved" (byte)
    /// </summary>
    [JsonPropertyName("isapproved")]
    public Types.ActionTypes IsApproved { get; set; }

    /// <summary>
    /// Indicates if the blog post is featured (1) or not (0)
    /// JSON Property: "isfeatured" (byte)
    /// </summary>
    [JsonPropertyName("isfeatured")]
    public Types.FeaturedTypes IsFeatured { get; set; }

    /// <summary>
    /// Draft status: 0 = Published, 1 = Draft
    /// JSON Property: "isdraft" (byte)
    /// </summary>
    [JsonPropertyName("isdraft")]
    public Types.DraftTypes IsDraft { get; set; }

    /// <summary>
    /// Archive status: 0 = Active, 1 = Archived
    /// JSON Property: "isarchive" (byte)
    /// </summary>
    [JsonPropertyName("isarchive")]
    public Types.ActionTypes IsArchive { get; set; } = Types.ActionTypes.Disabled;

    #endregion

    #region Engagement Metrics

    /// <summary>
    /// Total number of views for the blog post
    /// JSON Property: "views" (integer)
    /// </summary>
    [JsonPropertyName("views")]
    public int Views { get; set; }

    /// <summary>
    /// Total number of comments on the blog post
    /// JSON Property: "comments" (integer)
    /// </summary>
    [JsonPropertyName("comments")]
    public int Comments { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Author of the blog post (navigation property)
    /// JSON Property: "author" (object, nullable)
    /// </summary>
    [JsonPropertyName("author")]
    public ApplicationUser? Author { get; set; } = null!;

    #endregion

    #region NotMapped Properties

    /// <summary>
    /// Primary content data for the blog post (not mapped to database)
    /// JSON Property: "blog_data" (object, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("blog_data")]
    public BlogData? BlogData { get; set; }

    /// <summary>
    /// Localized content data for multiple cultures (not mapped to database)
    /// JSON Property: "blog_culture_data" (array, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("blog_culture_data")]
    public List<BlogData>? BlogCultureData { get; set; }

    /// <summary>
    /// Array of category IDs associated with the blog post (not mapped to database)
    /// JSON Property: "categories" (array, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("categories")]
    public int[]? Categories { get; set; }

    /// <summary>
    /// List of category items with details (not mapped to database)
    /// JSON Property: "categorylist" (array, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("categorylist")]
    public List<CategoryItem>? CategoryList { get; set; }

    /// <summary>
    /// Status indicator for UI actions (not mapped to database)
    /// JSON Property: "actionstatus" (string, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("actionstatus")]
    public string? ActionStatus { get; set; }

    /// <summary>
    /// Generated URL for the blog post (not mapped to database)
    /// JSON Property: "url" (string, nullable)
    /// </summary>
    [NotMapped]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    #endregion
}