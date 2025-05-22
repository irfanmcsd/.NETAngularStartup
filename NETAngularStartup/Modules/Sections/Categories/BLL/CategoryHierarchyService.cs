using DevCodeArchitect.DBContext;


namespace DevCodeArchitect.Entity;
public class CategoryHierarchyService
{
    public List<Categories> BuildHierarchy(List<Categories> categories, int? parentId = null)
    {
        var result = new List<Categories>();

        var levelCategories = categories
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.Priority)
            .ToList();

        foreach (var category in levelCategories)
        {
            category.Children = BuildHierarchy(categories, category.Id);
            result.Add(category);
        }

        return result;
    }
}
