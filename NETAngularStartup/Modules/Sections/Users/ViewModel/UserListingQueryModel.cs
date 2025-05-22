using DevCodeArchitect.DBContext;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Represents query parameters for user listing requests, typically bound from route or query string values.
/// Used for filtering, sorting, and paginating user listings in the application.
/// </summary>
public class UserListingQueryModel
{
    #region Search Parameters


    /// <summary>
    /// Gets or sets the broad search term for filtering users
    /// </summary>
    /// <example>"john"</example>
    [JsonProperty("term")]
    public string? Term { get; set; }

    /// <summary>
    /// Gets or sets the tracking indicator for search type (0=regular, 1=search)
    /// </summary>
    [JsonProperty("src")]
    public byte Source { get; set; }

    #endregion

    #region Company Parameters

    /// <summary>
    /// Gets or sets the company unique slug identifier
    /// </summary>
    /// <example>
    /// /agent-finder/company/xyzcompany => "xyzcompany"
    /// </example>
    [JsonProperty("companySlug")]
    public string? CompanySlug { get; set; }

    #endregion

    #region Category & Filtering

    /// <summary>
    /// Gets or sets the category identifier for filtering
    /// </summary>
    [JsonProperty("category")]
    public int? Category { get; set; }

    /// <summary>
    /// Gets or sets the featured status filter
    /// </summary>
    [JsonProperty("featured")]
    public Types.FeaturedTypes? Featured { get; set; }

    /// <summary>
    /// Gets or sets the additional filter parameter
    /// </summary>
    [JsonProperty("filter")]
    public string? Filter { get; set; }

    #endregion

    #region Sorting & Pagination

    /// <summary>
    /// Gets or sets the ordering clause for results
    /// </summary>
    /// <example>"name_asc"</example>
    [JsonProperty("order")]
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets the current page number for pagination
    /// </summary>
    [JsonProperty("pageNumber")]
    public int? PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the response type for the listing
    /// </summary>
    /// <remarks>
    /// Defaults to View mode
    /// </remarks>
    [JsonProperty("response")]
    public Types.ListResponse Response { get; set; } = Types.ListResponse.View;

    #endregion

    #region Location Parameters

    /// <summary>
    /// Gets or sets the location identifier
    /// </summary>
    [JsonProperty("locationId")]
    public int? LocationId { get; set; }

    /// <summary>
    /// Gets or sets the place search term
    /// </summary>
    [JsonProperty("placeTerm")]
    public string? PlaceTerm { get; set; }

    /// <summary>
    /// Gets or sets the city filter
    /// </summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state/province filter
    /// </summary>
    [JsonProperty("state")]
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country filter
    /// </summary>
    [JsonProperty("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the postal/zip code filter
    /// </summary>
    [JsonProperty("zip")]
    public string? Zip { get; set; }

    #endregion
}