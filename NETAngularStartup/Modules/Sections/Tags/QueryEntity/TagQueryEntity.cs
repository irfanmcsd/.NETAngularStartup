using Newtonsoft.Json;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Represents a query entity for searching and filtering tags
/// </summary>
/// <remarks>
/// Inherits from ContentEntity to provide common content-related properties.
/// Used primarily for tag autocomplete suggestions and filtered tag queries.
/// </remarks>
public class TagQueryEntity : ContentEntity
{
    /// <summary>
    /// Gets or sets the starting characters for tag search (autocomplete functionality)
    /// </summary>
    /// <example>
    /// If set to "pro", would match tags like "programming", "product", "professional"
    /// </example>
    [JsonProperty("startSearchKey")]
    public string? StartSearchKey { get; set; }

    /// <summary>
    /// Gets or sets the content type filter for tags
    /// </summary>
    /// <remarks>
    /// Defaults to Blog content type
    /// </remarks>
    [JsonProperty("type")]
    public TagEnum.Types Type { get; set; } = TagEnum.Types.Blog;

    /// <summary>
    /// Gets or sets the tag classification type filter
    /// </summary>
    /// <remarks>
    /// Defaults to Normal tags (non-system, non-search tags)
    /// </remarks>
    [JsonProperty("tagType")]
    public TagEnum.TagType TagType { get; set; } = TagEnum.TagType.Normal;

    /// <summary>
    /// Gets or sets the importance level filter for tags
    /// </summary>
    /// <remarks>
    /// Defaults to All levels (no filtering by importance)
    /// </remarks>
    [JsonProperty("tagLevel")]
    public TagEnum.TagLevel TagLevel { get; set; } = TagEnum.TagLevel.All;
}