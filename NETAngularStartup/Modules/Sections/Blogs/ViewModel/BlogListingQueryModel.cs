using DevCodeArchitect.DBContext;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Represents query parameters for blog listing and filtering.
/// This model binds with routing parameters and query strings to filter, sort, and paginate blog listings.
/// </summary>
public class BlogListingQueryModel
{
    /// <summary>
    /// Gets or sets the primary category name for filtering.
    /// Binds with route pattern: /blogs/category/{categoryname}
    /// Example: /blogs/category/technology
    /// </summary>
    public string? Culture { get; set; } = "en";
    /// <summary>
    /// Gets or sets the primary category name for filtering.
    /// Binds with route pattern: /blogs/category/{categoryname}
    /// Example: /blogs/category/technology
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the first-level subcategory for hierarchical filtering.
    /// Binds with route pattern: /blogs/category/{category_child1}/{categoryname}
    /// Example: /blogs/category/programming/csharp
    /// </summary>
    public string? CategoryChild1 { get; set; }

    /// <summary>
    /// Gets or sets the second-level subcategory for hierarchical filtering.
    /// Binds with route pattern: /blogs/category/{category_child2}/{category_child1}/{categoryname}
    /// </summary>
    public string? CategoryChild2 { get; set; }

    /// <summary>
    /// Gets or sets the third-level subcategory for hierarchical filtering.
    /// Binds with route pattern: /blogs/category/{category_child3}/.../{categoryname}
    /// </summary>
    public string? CategoryChild3 { get; set; }

    /// <summary>
    /// Gets or sets the fourth-level subcategory for hierarchical filtering.
    /// Binds with route pattern: /blogs/category/{category_child4}/.../{categoryname}
    /// </summary>
    public string? CategoryChild4 { get; set; }

    /// <summary>
    /// Gets or sets the search term for broad content search across multiple fields.
    /// Typically binds with query string parameter: ?term=searchphrase
    /// </summary>
    public string? Term { get; set; }

    /// <summary>
    /// Gets or sets the tag/label for filtering posts by specific tags.
    /// Typically binds with query string parameter: ?label=tagname
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the user profile slug for filtering posts by specific authors.
    /// Typically binds with route pattern: /blogs/author/{user_slug}
    /// </summary>
    public string? UserSlug { get; set; }

    /// <summary>
    /// Gets or sets the sorting order for the blog listing.
    /// Typically binds with query string parameter: ?order=fieldname
    /// Example values: "newest", "popular", "alphabetical"
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets additional filter criteria for specialized filtering.
    /// Typically binds with query string parameter: ?filter=criteria
    /// Example values: "featured", "recent", "commented"
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the page number for paginated results.
    /// Typically binds with query string parameter: ?pagenumber=2
    /// </summary>
    public int? PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the response format type for the listing.
    /// Defaults to View (standard HTML view).
    /// Other options include RSS, ATOM, JSON, etc.
    /// </summary>
    public Types.ListResponse Response { get; set; } = Types.ListResponse.View;

    /// <summary>
    /// Gets or sets the featured type filter for premium content.
    /// Can filter by Basic, Featured, Premium, or All posts.
    /// </summary>
    public Types.FeaturedTypes? Featured { get; set; }

    /// <summary>
    /// Gets or sets the search source identifier.
    /// Used to track whether the search originated from:
    /// 0 = Regular listing, 1 = Search box, 2 = Advanced search, etc.
    /// </summary>
    public byte SearchSource { get; set; }
}