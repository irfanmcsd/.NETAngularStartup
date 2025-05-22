using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.Entity;

public class TagEnum
{

    // prepare settings for front-end application
    public static object getSettings()
    {
        return new
        {
            // property settings
            types = getTypes(),
            tag_types = getTagTypes(),
            tag_levels = getTagLevel()
        };
    }

    // Convert Enum to Dictionary<string, int> and order by key

    public static List<KeyValuePair<string, int>> getTypes()
    {
        var _enumList = ((Types[])Enum.GetValues(typeof(Types)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }

    public static List<KeyValuePair<string, int>> getTagTypes()
    {
        var _enumList = ((TagType[])Enum.GetValues(typeof(TagType)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }

    public static List<KeyValuePair<string, int>> getTagLevel()
    {
        var _enumList = ((TagLevel[])Enum.GetValues(typeof(TagLevel)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }

    public enum Types
    {
        Blog = 0,
        Property = 1,
        Company = 2,
        All = 3
    }

    public enum TagType
    {
        Normal = 0,
        UserSearches = 1,
        All = 2
    }

    public enum TagLevel
    {
        High = 0,
        Medium = 1,
        Low = 2,
        All = 3
    }

}

/* Copyright © 2025, Mediasoftpro All rights reserved.
 * For inquiries and more information, please contact us at:
 * Email: clouddevarchitect@outlook.com
 * Website: www.devcodearchitect.com
 */