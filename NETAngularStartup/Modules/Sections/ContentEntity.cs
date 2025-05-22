using System.Text.Json.Serialization;
using static DevCodeArchitect.Entity.Types;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Base class for query parameters with common filtering, pagination, and sorting options.
/// Provides default values and standardized filtering capabilities across all content types.
/// </summary>
public class ContentEntity
{
    #region Localization & Identification

    /// <summary>
    /// Gets or sets the culture code for localized content filtering (e.g., "en-US").
    /// When set, only returns content matching the specified culture.
    /// </summary>
    [JsonPropertyName("culture")]
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets the slug for precise content lookup.
    /// Filters records to match either the exact term or sub-term (slug).
    /// </summary>
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the partial slug prefix for content lookup.
    /// Filters records where the term or sub-term starts with this value.
    /// </summary>
    [JsonPropertyName("slug_started_with")]
    public string? SlugStartedWith { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for direct content lookup.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    #endregion

    #region Authentication & Authorization

    /// <summary>
    /// Gets or sets whether the requesting user is authenticated.
    /// When false, may trigger additional public-only content filtering.
    /// Default: false
    /// </summary>
    [JsonPropertyName("user_signin")]
    public bool UserSignIn { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the request originates from an admin interface.
    /// When true, may bypass certain content restrictions.
    /// Default: false
    /// </summary>
    [JsonPropertyName("isadmin")]
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// Gets or sets the user ID for personalized content filtering.
    /// Used with properties like LoadFavorites or LoadLiked.
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    #endregion

    #region Pagination & Sorting

    /// <summary>
    /// Gets or sets the current page number for paginated results.
    /// Default: 1
    /// </summary>
    [JsonPropertyName("pagenumber")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// Default: 18
    /// </summary>
    [JsonPropertyName("pagesize")]
    public int PageSize { get; set; } = 18;

    /// <summary>
    /// Gets or sets whether to load all records ignoring pagination.
    /// When true, PageNumber and PageSize are ignored.
    /// Default: false
    /// </summary>
    [JsonPropertyName("loadall")]
    public bool LoadAll { get; set; } = false;

    /// <summary>
    /// Gets or sets the SQL ORDER BY clause for sorting results.
    /// Example: "title ASC" or "created_at DESC"
    /// </summary>
    [JsonPropertyName("order")]
    public string? Order { get; set; }

    #endregion

    #region Content Filtering

    /// <summary>
    /// Gets or sets the ID to exclude from results.
    /// When > 0, excludes the specified record from results.
    /// </summary>
    [JsonPropertyName("exludedid")]
    public int ExcludedId { get; set; }

    /// <summary>
    /// Gets or sets the category ID for content filtering.
    /// </summary>
    [JsonPropertyName("categoryid")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category name for content filtering.
    /// </summary>
    [JsonPropertyName("categoryname")]
    public string? CategoryName { get; set; }

    /// <summary>
    /// Gets or sets an array of category IDs for multi-category filtering.
    /// </summary>
    [JsonPropertyName("category_ids")]
    public int[]? CategoryIds { get; set; }

    /// <summary>
    /// Gets or sets an array of content IDs for specific record retrieval.
    /// </summary>
    [JsonPropertyName("range_ids")]
    public int[]? RangeIds { get; set; }

    /// <summary>
    /// Gets or sets the general search term for broad content filtering.
    /// Typically searches across multiple fields (title, description, etc.).
    /// </summary>
    [JsonPropertyName("term")]
    public string? Term { get; set; }

    /// <summary>
    /// Gets or sets tags for content filtering (comma-separated values).
    /// </summary>
    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    #endregion

    #region Date Filtering

    /// <summary>
    /// Gets or sets the start date for date range filtering.
    /// </summary>
    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for date range filtering.
    /// </summary>
    [JsonPropertyName("end_date")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the month for temporal filtering (1-12).
    /// </summary>
    [JsonPropertyName("month")]
    public int Month { get; set; }

    /// <summary>
    /// Gets or sets the year for temporal filtering.
    /// </summary>
    [JsonPropertyName("year")]
    public int Year { get; set; }

    /// <summary>
    /// Gets or sets the date filtering strategy.
    /// Default: DateFilter.All (no date filtering)
    /// </summary>
    [JsonPropertyName("dateFilter")]
    public DateFilter DateFilter { get; set; } = DateFilter.All;

    #endregion

    #region Content Status Flags

    /// <summary>
    /// Gets or sets whether to filter for public content only.
    /// When true, considers multiple status flags (enabled, approved, etc.).
    /// Default: true
    /// </summary>
    [JsonPropertyName("ispublic")]
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Gets or sets the enabled status filter.
    /// Default: ActionTypes.Enabled (only enabled content)
    /// </summary>
    [JsonPropertyName("isenabled")]
    public ActionTypes IsEnabled { get; set; } = ActionTypes.Enabled;

    /// <summary>
    /// Gets or sets the approval status filter.
    /// Default: ActionTypes.Enabled (only approved content)
    /// </summary>
    [JsonPropertyName("isapproved")]
    public ActionTypes IsApproved { get; set; } = ActionTypes.Enabled;

    /// <summary>
    /// Gets or sets the draft status filter.
    /// Default: ActionTypes.Disabled (exclude drafts)
    /// </summary>
    [JsonPropertyName("isdraft")]
    public DraftTypes IsDraft { get; set; } = DraftTypes.Normal;
    /// <summary>
   
    /// <summary>
    /// Gets or sets the archive status filter.
    /// Default: ActionTypes.Disabled (exclude archived content)
    /// </summary>
    [JsonPropertyName("isarchive")]
    public ActionTypes IsArchive { get; set; } = ActionTypes.Disabled;

    /// <summary>
    /// Gets or sets the archive/expiry filtering option.
    /// Default: ArchiveExpiryOptions.All (no archive/expiry filtering)
    /// </summary>
    [JsonPropertyName("archive_expiry")]
    public ArchiveExpiryOptions ArchiveExpiry { get; set; } = ArchiveExpiryOptions.All;

    #endregion

    #region Performance & Caching

    /// <summary>
    /// Gets or sets whether to use caching for the query.
    /// Default: false
    /// </summary>
    [JsonPropertyName("iscache")]
    public bool IsCache { get; set; }

    /// <summary>
    /// Gets or sets whether to skip record count statistics.
    /// When true, improves performance by avoiding COUNT queries.
    /// Default: false
    /// </summary>
    [JsonPropertyName("skip_record_stats")]
    public bool skip_record_stats { get; set; } = false;

    #endregion

    #region Advanced Features

    /// <summary>
    /// Gets or sets whether to apply advanced filtering.
    /// When false, uses only basic filters (ID, slug).
    /// Default: true
    /// </summary>
    [JsonPropertyName("advancefilter")]
    public bool AdvanceFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to load user favorites (requires UserId).
    /// Default: false
    /// </summary>
    [JsonPropertyName("loadfavorites")]
    public bool LoadFavorites { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to load analytics data with content.
    /// Default: false
    /// </summary>
    [JsonPropertyName("loadanalytics")]
    public bool LoadAnalytics { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to load user liked/rated content (requires UserId).
    /// Default: false
    /// </summary>
    [JsonPropertyName("loadliked")]
    public bool LoadLiked { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to include dynamic attributes in results.
    /// Default: false
    /// </summary>
    [JsonPropertyName("mawithDynamicAttrs")]
    public bool MapWithDynamicAttrs { get; set; } = false;

    /// <summary>
    /// Gets or sets the configuration for dynamic attribute mapping.
    /// Used when MapWithDynamicAttrs is true.
    /// </summary>
    [JsonPropertyName("map_terms")]
    public List<DynamicAttrsMapObject>? MapTerms { get; set; }

    /// <summary>
    /// Gets or sets whether to generate report data instead of standard listings.
    /// Default: false
    /// </summary>
    [JsonPropertyName("render_report")]
    public bool RenderReport { get; set; } = false;

    /// <summary>
    /// Gets or sets which columns to fetch based on view requirements.
    /// Default: FetchColumnOptions.List (standard list view columns)
    /// </summary>
    [JsonPropertyName("column_options")]
    public FetchColumnOptions ColumnOptions { get; set; } = FetchColumnOptions.List;

    #endregion
}
