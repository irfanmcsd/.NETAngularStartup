using DevCodeArchitect.DBContext;

namespace DevCodeArchitect.Entity;
public class CategoryNavItemModel
{
    public Categories? Item { get; set; }
    public string? Url { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public bool IsFirstParent { get; set; }
    public string? CurrentTerm { get; set; }
    public string? Path { get; set; } = "/doc";
}