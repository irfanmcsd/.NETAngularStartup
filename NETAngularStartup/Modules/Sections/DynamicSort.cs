
using System.Linq.Dynamic.Core;
namespace DevCodeArchitect.Utilities;

public static class DynamicSort
{
    public static IQueryable Sort(this IQueryable collection, string sortBy, bool reverse = false)
    {
        return collection.OrderBy(sortBy + (reverse ? " descending" : ""));
    }
}
