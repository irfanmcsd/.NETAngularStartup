
using DevCodeArchitect.DBContext;

namespace DevCodeArchitect.Entity;
public class BlogListViewModel : ListViewModel
{
    /// <summary>
    /// Total no of records exist
    /// </summary>
    public int? TotalRecords { get; set; }

    /// <summary>
    /// List of record returned
    /// </summary>
    public List<Blogs>? DataList { get; set; }

    /// <summary>
    /// List of data in string format (RSS, ATOM, Goolge, Bing Sitemaps)
    /// </summary>
    public string? DataStr { get; set; }

    /// <summary>
    /// Query entity for query builder
    /// </summary>
    public BlogQueryEntity? QueryOptions { get; set; }
}
