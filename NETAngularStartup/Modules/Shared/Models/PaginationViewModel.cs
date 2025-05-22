namespace DevCodeArchitect.Utilities;
public class PaginationViewModel
{
    public int pagenumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalRecords { get; set; } = 0;
    public string Default_Url { get; set; } = string.Empty;
    public string Pagination_Url { get; set; } = string.Empty;
    public bool isFilter { get; set; } = false;
    public string Filter_Default_Url { get; set; } = string.Empty;
    public string Filter_Pagination_url { get; set; } = string.Empty;
    public bool ShowFirst { get; set; } = true;
    public bool ShowLast { get; set; } = true;
}