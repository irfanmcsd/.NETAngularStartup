using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.Entity;

public class RoleEnum
{
    // prepare settings for front-end application
    public static object getSettings()
    {
        return new
        {
            // property settings
            types = getTypes()
        };
    }

    public static List<KeyValuePair<string, int>> getTypes()
    {
        var _enumList = ((Types[])Enum.GetValues(typeof(Types)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }


    public enum Types
    {
        Admin = 0,
        Company = 1,
        All =2
    }

}