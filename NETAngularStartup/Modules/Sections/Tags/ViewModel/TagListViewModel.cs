using DevCodeArchitect.DBContext;
using System.Collections.Generic;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Represents a view model for displaying lists of tags with pagination and serialization support.
/// Inherits from the base ListViewModel class to provide common list functionality.
/// </summary>
public class TagListViewModel : ListViewModel
{
    /// <summary>
    /// Gets or sets the total number of records available in the data source.
    /// This count represents all matching records before pagination is applied.
    /// </summary>
    public int? TotalRecords { get; set; }

    /// <summary>
    /// Gets or sets the list of tag records returned by the query.
    /// This collection represents the current page of data when pagination is used.
    /// </summary>
    public List<Tags>? DataList { get; set; }

    /// <summary>
    /// Gets or sets the serialized string representation of the data.
    /// Used for alternative output formats like RSS, ATOM, or XML sitemaps.
    /// </summary>
    public string? DataStr { get; set; }

    /// <summary>
    /// Gets or sets the query parameters that were used to generate this view model.
    /// Contains all filtering, sorting, and pagination options.
    /// </summary>
    public TagQueryEntity? QueryOptions { get; set; }
}