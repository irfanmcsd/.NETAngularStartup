using System.Text.RegularExpressions;

public static class HtmlSanitizer
{
    // Compile regex patterns once for better performance
    private static readonly Regex _scriptRegex = new Regex(
        @"<script[^>]*?>.*?</script>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex _styleRegex = new Regex(
        @"<style[^>]*?>.*?</style>",
        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex _tagRegex = new Regex(
        @"<[^>]+>",
        RegexOptions.Compiled);

    private static readonly Regex _commentRegex = new Regex(
        @"<![\s\S]*?--[ \t\n\r]*>",
        RegexOptions.Compiled);

    private static readonly Regex _bbCodeRegex = new Regex(
        @"\[(\w)+\](.+?)\[/(\w)+\]",
        RegexOptions.Compiled);

    /// <summary>
    /// Removes all HTML tags, scripts, styles, comments and BBCode from input string
    /// </summary>
    /// <param name="input">HTML content to sanitize</param>
    /// <returns>Plain text with all markup removed</returns>
    public static string StripHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Decode HTML entities first to handle encoded content
        string sanitized = System.Net.WebUtility.HtmlDecode(input);

        // Apply removal patterns in specific order for maximum effectiveness
        sanitized = _scriptRegex.Replace(sanitized, string.Empty);
        sanitized = _styleRegex.Replace(sanitized, string.Empty);
        sanitized = _commentRegex.Replace(sanitized, string.Empty);
        sanitized = _tagRegex.Replace(sanitized, string.Empty);
        sanitized = _bbCodeRegex.Replace(sanitized, string.Empty);

        // Normalize whitespace that might result from tag removal
        sanitized = Regex.Replace(sanitized, @"\s+", " ").Trim();

        return sanitized;
    }
}