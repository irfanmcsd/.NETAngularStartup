using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Provides category-related enumerations and utility methods for working with category types.
/// </summary>
public static class CategoryEnum
{
    /// <summary>
    /// Enumeration defining different category types in the system.
    /// Each type represents a distinct content classification.
    /// </summary>
    public enum Types
    {

        /// <summary>
        /// Represents property listings (value: 1)
        /// </summary>
        Properties = 1,

        /// <summary>
        /// Represents blog posts and articles (value: 2)
        /// </summary>
        Blogs = 2,

        /// <summary>
        /// Represents company profiles (value: 3)
        /// </summary>
        Companies = 3,

        /// <summary>
        /// Represents rental properties (value: 4)
        /// </summary>
        Properties_Rent = 4,

        /// <summary>
        /// Represents categories (value: 5)
        /// </summary>
        Categories = 5,

        /// <summary>
        /// Represents users (value: 6)
        /// </summary>
        Users = 6,

        /// <summary>
        /// Represents documents (value: 7)
        /// </summary>
        Documentation = 7,


    }

    /// <summary>
    /// Gets all category type settings formatted for front-end consumption.
    /// </summary>
    /// <returns>
    /// An anonymous object containing categorized enumerations.
    /// Structure: { types: List&lt;KeyValuePair&lt;string, int&gt;&gt; }
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            // Category type definitions
            types = GetTypes()
        };
    }

    /// <summary>
    /// Retrieves all category types as an ordered list of key-value pairs.
    /// </summary>
    /// <returns>
    /// List of KeyValuePair where:
    /// - Key: The enum name as string
    /// - Value: The enum integer value
    /// </returns>
    public static List<KeyValuePair<string, int>> GetTypes()
    {
        var enumValues = ((Types[])Enum.GetValues(typeof(Types)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();

        return UtilityHelper.EnumOrderBy(enumValues);
    }
}