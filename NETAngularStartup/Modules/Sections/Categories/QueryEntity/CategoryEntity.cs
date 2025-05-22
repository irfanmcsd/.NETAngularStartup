namespace DevCodeArchitect.Entity;

public class CategoryQueryEntity : ContentEntity
{
    public int parentid { get; set; } = -1;
    public string? start_search_key { get; set; }
    public CategoryEnum.Types type { get; set; } = CategoryEnum.Types.Properties;
    public Types.FeaturedTypes isfeatured { get; set; } = Types.FeaturedTypes.All;
}
