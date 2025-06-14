using DevCodeArchitect.Settings;
using System;
using System.Text;
using System.Text.RegularExpressions;
using TimeZoneConverter;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides general utility methods for string manipulation, formatting, and common operations.
/// </summary>
public static class UtilityHelper
{
    #region String Manipulation

    public static string RemoveCulturePrefix(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string pattern = @"^[a-zA-Z][a-zA-Z-]+/";
        return Regex.Replace(input, pattern, string.Empty);
    }

    public static string StripCurlyBraces(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove all curly braces (both surrounding and internal)
        return Regex.Replace(input, "[{}]", "");
    }

    /// <summary>
    /// Converts a string into a URL-friendly slug
    /// </summary>
    /// <param name="input">Input string to convert</param>
    /// <param name="length">Maximum length of the resulting slug (0 for no limit)</param>
    /// <returns>URL-friendly slug string</returns>
    public static string PrepareSlug(string input, int length = 0)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var output = ReplaceSpaceWithHyphen(input);

        if (string.IsNullOrEmpty(output))
            return string.Empty;

        output = output.ToLower().Trim();

        return length > 0 && output.Length > length
            ? output[..length]
            : output;
    }

    /// <summary>
    /// Replaces spaces with hyphens and removes special characters from a string
    /// </summary>
    /// <param name="input">Input string to process</param>
    /// <returns>Processed string with hyphens instead of spaces</returns>
    public static string ReplaceSpaceWithHyphen(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var str = input
            .Replace("&amp;", "-")
            .Replace(" ", "-");

        str = Regex.Replace(str, @"[\-]+", "-");
        str = Regex.Replace(str, @"[^0-9a-zA-Z\-_.]+", "");

        return str;
    }

    public static string ReplaceHyphensWithSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Replace various hyphen/dash characters
        return input
            .Replace('-', ' ')     // Regular hyphen
            .Replace('–', ' ')     // En-dash
            .Replace('—', ' ');    // Em-dash
    }

    /// <summary>
    /// Removes all special characters (including spaces) from a string
    /// </summary>
    /// <param name="input">Input string to process</param>
    /// <returns>String containing only alphanumeric characters</returns>
    public static string RemoveSpecialChars(string? input)
    {
        return string.IsNullOrEmpty(input)
            ? string.Empty
            : Regex.Replace(input, @"[^0-9a-zA-Z]+", "");
    }

    /// <summary>
    /// Converts a string to title case (capitalizes first letter of each word)
    /// </summary>
    /// <param name="s">Input string to convert</param>
    /// <param name="allWords">Whether to capitalize all words or just the first word</param>
    /// <returns>Title-cased string</returns>
    public static string ToTitleCase(string? s, bool allWords = true)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        if (!allWords)
            return CapitalizeFirstLetter(s);

        var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (result.Length > 0)
                result.Append(' ');

            result.Append(CapitalizeFirstLetter(word));
        }

        return result.ToString();
    }

    /// <summary>
    /// Capitalizes the first letter of a string
    /// </summary>
    /// <param name="s">Input string</param>
    /// <returns>String with first letter capitalized</returns>
    public static string CapitalizeFirstLetter(string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;

        return char.ToUpper(s[0]) + s[1..];
    }

    /// <summary>
    /// Removes all HTML tags and scripts from a string
    /// </summary>
    /// <param name="input">HTML string to sanitize</param>
    /// <returns>Plain text without HTML markup</returns>
    public static string StripHtml(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove scripts, styles, HTML tags, comments, and BBCode
        return Regex.Replace(input, @"
            <script[^>]*?>.*?</script> |  # Script tags
            <[\/\!]*?[^<>]*?>          |  # HTML tags
            <style[^>]*?>.*?</style>   |  # Style tags
            <![\s\S]*?--[ \t\n\r]*>    |  # Comments
            \[(\w)+\](.+)\[/(\w)+\]       # BBCode
        ", string.Empty, RegexOptions.IgnorePatternWhitespace);
    }

    #endregion

    #region Path and Directory Helpers

    /// <summary>
    /// Normalizes directory paths by ensuring proper slash formatting
    /// </summary>
    /// <param name="path">Path to normalize</param>
    /// <returns>Normalized path with consistent slashes</returns>
    public static string NormalizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        // Remove leading slash if present
        if (path.StartsWith("/"))
            path = path[1..];

        // Ensure trailing slash
        if (!path.EndsWith("/"))
            path += "/";

        return path;
    }

    /// <summary>
    /// Prepares a directory name by removing special characters and normalizing
    /// </summary>
    /// <param name="dirName">Directory name to process</param>
    /// <returns>Sanitized directory name</returns>
    public static string PrepareDirectoryName(string? dirName)
    {
        if (string.IsNullOrEmpty(dirName))
            return "media/";

        // Replace spaces with underscores and remove special chars
        var sanitized = Regex.Replace(dirName.Trim().ToLower(),
            @"\s", "_");
        sanitized = Regex.Replace(sanitized,
            @"[^0-9a-zA-Z_\/]+", string.Empty);

        return NormalizePath(sanitized);
    }

    #endregion

    #region Random Generators

    /// <summary>
    /// Generates a random number within a specified range
    /// </summary>
    /// <param name="min">Minimum value (inclusive)</param>
    /// <param name="max">Maximum value (exclusive)</param>
    /// <returns>Random integer</returns>
    public static int RandomNumber(int min, int max)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max");

        return Random.Shared.Next(min, max);
    }

    /// <summary>
    /// Generates a random alphanumeric string
    /// </summary>
    /// <param name="length">Length of the string to generate</param>
    /// <returns>Random string</returns>
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Shared.Next(s.Length)])
            .ToArray());
    }

    /// <summary>
    /// Generates a URL-friendly slug with a random suffix
    /// </summary>
    /// <param name="name">Base name for the slug</param>
    /// <returns>Generated slug with random suffix</returns>
    public static string GenerateSlug(string? name)
    {
        var baseName = string.IsNullOrEmpty(name)
            ? RandomString(20)
            : RemoveSpecialChars(name.ToLower()[..Math.Min(name.Length, 20)]);

        return $"{baseName}{RandomNumber(2222, 9999)}";
    }

    #endregion

    #region Role and Permission Helpers

    /// <summary>
    /// Checks if a role exists in a list of roles
    /// </summary>
    /// <param name="roles">List of roles to check</param>
    /// <param name="roleName">Role name to find</param>
    /// <returns>True if the role exists, false otherwise</returns>
    public static bool HasRole(IList<string>? roles, string roleName)
    {
        return roles?.Contains(roleName) ?? false;
    }

    #endregion

    #region Cookie Helpers

    /// <summary>
    /// Writes a cookie to the HTTP response
    /// </summary>
    /// <param name="key">Cookie name</param>
    /// <param name="value">Cookie value</param>
    public static void WriteCookie(string key, string value)
    {
        if (SiteConfigs.HttpContextAccessor?.HttpContext != null)
        {
            SiteConfigs.HttpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
        }
    }

    /// <summary>
    /// Reads a cookie from the HTTP request
    /// </summary>
    /// <param name="name">Cookie name to read</param>
    /// <returns>Cookie value or null if not found</returns>
    public static string? ReadCookie(string name)
    {
        return SiteConfigs.HttpContextAccessor?.HttpContext?.Request.Cookies[name];
    }

    #endregion

    #region Boolean Helpers

    /// <summary>
    /// Safely checks if a nullable boolean is true
    /// </summary>
    /// <param name="value">Nullable boolean value</param>
    /// <returns>True if value is non-null and true, false otherwise</returns>
    public static bool IsTrue(bool? value)
    {
        return value ?? false;
    }

    public static string AdjustSlash(string? file)
    {
        if (file == null)
            return "";

        // remove '/' from start if exist in path
        if (file.StartsWith("/"))
            file = file.Remove(0, 1);

        // add '/' if not exist at the end of file
        if (!file.EndsWith("/"))
            file = file + "/";

        return file;
    }

    public static string PrepareDirName(string? dir_name)
    {
        if (!string.IsNullOrEmpty(dir_name))
        {
            // strip special characters except slash '/' and replace space with underscore
            var str = Regex.Replace(dir_name.Trim().ToLower(), "\\s", "_");
            str = Regex.Replace(str, @"[^0-9a-zA-Z_\/]+", "");

            if (str.StartsWith("/"))
                str = str.Remove(0, 1);

            if (!str.EndsWith("/"))
                str = str + "/";

            return str;
        }

        return "media/";
    }

    public static DateTime TimeZoneOffsetDateTime()
    {
        const string defaultTimeZone = "Etc/UTC"; // Fallback to UTC if configuration is missing

        var timeZoneId = !string.IsNullOrEmpty(ApplicationSettings.Localization.Timezone)
            ? ApplicationSettings.Localization.Timezone
            : defaultTimeZone;

        try
        {
            // Convert IANA timezone to Windows timezone if needed
            var tzInfo = TZConvert.GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo);
        }
        catch (TimeZoneNotFoundException)
        {
            // Log error here and fallback to UTC
            return DateTime.UtcNow;
        }
    }

    public static bool isInRole(IList<string>? roles, string roleName)
    {
        if (roles != null)
        {
            foreach (var role in roles)
            {
                if (role == roleName)
                    return true;
            }
        }
        return false;
    }

    public static bool IsSelected(bool? value)
    {
        if (value != null && (bool)value)
            return true;
        else
            return false;
    }

    public static string SetTitle(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return UpperCaseFirst(ReplaceSpaceWithHyphen(title), true);
        else
            return "";
    }

    public static string UpperCaseFirst(string? s, bool isallcharacters = true)
    {
        if (s == null)
            return "";

        var str = new StringBuilder();
        if (isallcharacters)
        {
            if (s.Contains(" "))
            {
                string[] arr = s.ToString().Split(char.Parse(" "));
                int i = 0;
                for (i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i].ToString().Length > 0)
                    {
                        str.Append(UpperCaseFirst(arr[i].ToString()) + Convert.ToString(" "));
                    }
                }
            }
            else
            {
                str.Append(UpperCaseFirst(s));
            }
        }
        else
        {
            str.Append(UpperCaseFirst(s));
        }
        return str.ToString();
    }

    public static string UpperCaseFirst(string? s)
    {
        if (s == null)
            return "";
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static List<KeyValuePair<string, int>> EnumOrderBy(List<KeyValuePair<string, int>> list)
    {
        return list.OrderBy(x => x.Key).ToList();
    }

    public static string Add_pagenumber(string? input, string? value)
    {
        if (input == null)
            return "";
        if (value == null)
            value = "1";

        return Regex.Replace(input, "\\[p\\]", value);
    }

    #endregion
}