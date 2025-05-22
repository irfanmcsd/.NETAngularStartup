using DevCodeArchitect.Utilities;
using static DevCodeArchitect.Entity.TagEnum;
using static DevCodeArchitect.Entity.UserEnum;

namespace DevCodeArchitect.Entity
{
    /// <summary>
    /// Provides type definitions and utility methods for various application enumerations and settings.
    /// This class serves as a central repository for enum types and their related operations.
    /// </summary>
    public class Types
    {
        /// <summary>
        /// Gets a consolidated settings object containing all front-end configuration options.
        /// </summary>
        /// <returns>An anonymous object containing grouped settings for featured types, expiry options, 
        /// date filters, and chart grouping options.</returns>
        public static object GetSettings()
        {
            return new
            {
                // Property settings grouped for front-end consumption
                featured = GetFeaturedTypes(),
                expiry_options = GetExpiryOptions(),
                datefilter = GetDateFilterOptions(),
                groupby = GetChartGroupByOptions()
            };
        }

        #region Enum Retrieval Methods

        /// <summary>
        /// Gets all FeaturedTypes enum values as an ordered list of key-value pairs.
        /// </summary>
        public static List<KeyValuePair<string, int>> GetFeaturedTypes()
        {
            var enumList = ((FeaturedTypes[])Enum.GetValues(typeof(FeaturedTypes)))
                .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
                .ToList();
            return UtilityHelper.EnumOrderBy(enumList);
        }

        /// <summary>
        /// Gets all ExpiryOptions enum values as an ordered list of key-value pairs.
        /// </summary>
        public static List<KeyValuePair<string, int>> GetExpiryOptions()
        {
            var enumList = ((ExpiryOptions[])Enum.GetValues(typeof(ExpiryOptions)))
                .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
                .ToList();
            return UtilityHelper.EnumOrderBy(enumList);
        }

        /// <summary>
        /// Gets all DateFilter enum values as an ordered list of key-value pairs.
        /// </summary>
        public static List<KeyValuePair<string, int>> GetDateFilterOptions()
        {
            var enumList = ((DateFilter[])Enum.GetValues(typeof(DateFilter)))
                .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
                .ToList();
            return UtilityHelper.EnumOrderBy(enumList);
        }

        /// <summary>
        /// Gets all ChartGroupBy enum values as an ordered list of key-value pairs.
        /// </summary>
        public static List<KeyValuePair<string, int>> GetChartGroupByOptions()
        {
            var enumList = ((ChartGroupBy[])Enum.GetValues(typeof(ChartGroupBy)))
                .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
                .ToList();
            return UtilityHelper.EnumOrderBy(enumList);
        }

        #endregion

        #region Enum Parsing Utility

        /// <summary>
        /// Converts a FeaturedTypes enum value to its lowercase string representation.
        /// </summary>
        /// <param name="featured">The FeaturedTypes enum value to parse</param>
        /// <returns>Lowercase string representation or "all" if null</returns>
        public static string ParseFeaturedTypes(FeaturedTypes? featured)
        {
            if (featured != null)
            {
                var value = Enum.GetName(typeof(FeaturedTypes), featured);
                return string.IsNullOrEmpty(value) ? "all" : value.ToLower();
            }
            return "all";
        }

        #endregion

        #region Enum Definitions

        /// <summary>
        /// Defines response format options for list data
        /// </summary>
        public enum ListResponse
        {
            View,
            RSS,
            ATOM,
            Google,
            Bing,
            JSON,
            Half_Map,
            Full_Map
        }

        /// <summary>
        /// Defines content featuring levels
        /// </summary>
        public enum FeaturedTypes
        {
            Basic = 0,
            Featured = 1,
            Premium = 2,
            All = 3
        }

        /// <summary>
        /// Defines content expiration timeframes
        /// </summary>
        public enum ExpiryOptions
        {
            Expired = 1,
            ExpireToday = 2,
            Expire_in_5Days = 3,
            Expire_in_10Days = 6,
            Expire_in_Month = 5,
            All = 4
        }

        /// <summary>
        /// Defines date filtering options
        /// </summary>
        public enum DateFilter
        {
            Today = 1,
            ThisWeek = 2,
            ThisMonth = 3,
            ThisYear = 4,
            ThisHour = 5,
            CurrentPrevMonth = 6,
            PrevMonth = 7,
            PrevThreeMonths = 8,
            PrevSixMonths = 9,
            LastSixHour = 10,
            Last12Months = 11,
            Last24Months = 12,
            PrevYear = 13,
            All = 0
        }

        /// <summary>
        /// Defines simplified expiration options for archived content
        /// </summary>
        public enum ArchiveExpiryOptions
        {
            Expired = 1,
            ExpireToday = 2,
            Expire_in_5Days = 3,
            All = 4
        }

        /// <summary>
        /// Defines data fetch modes for UI presentation
        /// </summary>
        public enum FetchColumnOptions
        {
            List = 1,
            Dropdown = 2,
            Profile = 3
        }

        /// <summary>
        /// Defines possible action states
        /// </summary>
        public enum ActionTypes
        {
            Disabled = 0,
            Enabled = 1,
            Archive = 3,
            Locked = 4,
            All = 2
        }

        /// <summary>
        /// Defines possible draft states
        /// </summary>
        public enum DraftTypes
        {
            Normal = 0,
            Draft = 1,
            All = 2
        }

        /// <summary>
        /// Defines data grouping options for chart visualization
        /// </summary>
        public enum ChartGroupBy
        {
            Hour = 0,
            Day = 1,
            Month = 2,
            Year = 3,
            Country = 4,
            Location = 5,
            Categories = 6,
            None = 7
        }

        #endregion
    }

    /// <summary>
    /// Represents a dynamic attribute mapping object with condition evaluation
    /// </summary>
    public class DynamicAttrsMapObject
    {
        /// <summary>
        /// Gets or sets the attribute key
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the comparison condition (default: Match)
        /// </summary>
        public MappingConditions Condition { get; set; } = MappingConditions.Match;
    }

    /// <summary>
    /// Defines conditions for dynamic attribute matching
    /// </summary>
    public enum MappingConditions
    {
        Match = 0,
        Less_Than = 1,
        Greater_Than = 2,
        Exist = 3,
        Boolean = 4
    }
}