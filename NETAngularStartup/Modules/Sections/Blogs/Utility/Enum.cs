using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.Entity;

public class BlogEnum
{
    // prepare settings for front-end application
    public static object getSettings()
    {
        return new
        {
            // property settings
            groupby = getChartGroupBy()
        };
    }

    // Convert Enum to Dictionary<string, int> and order by key

    public static List<KeyValuePair<string, int>> getChartGroupBy()
    {
        var _enumList = ((ChartGroupBy[])Enum.GetValues(typeof(ChartGroupBy)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }

    public enum ChartGroupBy
    {
        Hour = 0,
        Day = 1,
        Month = 2,
        Year = 3,
        Categories = 4,
        None = 5
    }

}

/* Copyright © 2025, Mediasoftpro All rights reserved.
 * For inquiries and more information, please contact us at:
 * Email: clouddevarchitect@outlook.com
 * Website: www.devcodearchitect.com
 */