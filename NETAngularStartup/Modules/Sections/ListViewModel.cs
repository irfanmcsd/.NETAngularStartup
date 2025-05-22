namespace DevCodeArchitect.Entity;
/// <summary>
/// This class will be shared / inherited by all view models that handles listings
/// </summary>
public class ListViewModel
{
    /// <summary>
    /// Set main heading title (listing)
    /// </summary>
    public string? HeadingTitle { get; set; }

    /// <summary>
    /// Set second layer heading title (listing)
    /// </summary>
    public string? SubHeadingTitle { get; set; }


    /// <summary>
    /// Set description, normally display inbetween heading and listing
    /// </summary>
    public string? HeadingDescription { get; set; }

    /// <summary>
    /// Toggle on | off stats on top or bottom of listings
    /// </summary>
    public bool IsListStats { get; set; }

    /// <summary>
    /// Toggle on | off list filter navs normally appear on top of listings e.g order / filter dropdown
    /// </summary>
    public bool IsListNav { get; set; }

    /// <summary>
    /// Set custom text message to be appeared if no records found
    /// </summary>
    public string? NoRecordFoundText { get; set; }

    /// <summary>
    /// Set listing page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Set default url for pagination purpose
    /// </summary>
    public string? DefaultUrl { get; set; }

    /// <summary>
    /// Set pagination url pattern
    /// </summary>
    public string? PaginationUrl { get; set; }

    /// <summary>
    /// Set browse button text message e.g "Browse All"
    /// </summary>
    public string? BrowseText { get; set; }

    /// <summary>
    /// Set browse url
    /// </summary>
    public string? BrowseUrl { get; set; }

    /// <summary>
    /// Toggle on | off pagination
    /// </summary>
    public bool EnablePagination { get; set; }

    /// <summary>
    /// Set list of bread items appear above main listings
    /// </summary>
    public List<BreadItem>? BreadItems { get; set; }

    /// <summary>
    /// Set default listing type to render data in template
    /// </summary>
    public ListType ListType { get; set; } = ListType.List;

    public string? SelectedOrder { get; set; }


}

public enum ListType {
    Grid = 0,
    List = 1,
    Links = 2,
    Map_Half = 3,
    Map_Full =4
}
/// <summary>
/// Core model for handling breadcrum on pages
/// </summary>
public class BreadViewModel
{
    /// <summary>
    ///  Set title appear on top of breadcrum
    /// </summary>
    public string? HeaderTitle { get; set; }

    /// <summary>
    /// Set one or more bread items
    /// </summary>
    public List<BreadItem>? BreadItems { get; set; }
}

/// <summary>
/// Class to be used for generating bread items
/// </summary>
public class BreadItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Url { get; set; }
}
