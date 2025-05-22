using DevCodeArchitect.Settings;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides centralized management for system directory paths and URL generation.
/// This utility class handles path construction for file uploads, thumbnails, and media content access.
/// </summary>
/// <remarks>
/// All paths are designed to work both locally and in cloud environments (like AWS Lambda).
/// Paths should be configured to match deployment environment requirements.
/// </remarks>
public class SystemDirectoryPaths
{
    #region Default Directory Paths

    /// <summary>
    /// Gets or sets the default directory path for saving uploaded media contents.
    /// Default value: "/wwwroot/contents/temp/"
    /// </summary>
    public static string UploadDirectoryPath { get; set; } = "/wwwroot/contents/temp/";

    /// <summary>
    /// Gets or sets the default directory URL for accessing uploaded media contents.
    /// Default value: "contents/temp/"
    /// </summary>
    public static string UploadDirectoryUrl { get; set; } = "contents/temp/";

    /// <summary>
    /// Gets or sets the directory path for Lambda-generated thumbnails.
    /// Important: This must match AWS Lambda configuration.
    /// Default value: "photos/thumbs/"
    /// </summary>
    /// <warning>
    /// Changing this value requires corresponding updates in AWS Lambda scripts
    /// to prevent thumbnail generation issues.
    /// </warning>
    public static string LambdaThumbnailDirectory { get; set; } = "photos/thumbs/";

    #endregion

    #region Path Preparation Methods

    /// <summary>
    /// Prepares a complete upload directory path by combining environment root with user-specific directory.
    /// </summary>
    /// <param name="userId">Optional user identifier for user-specific directories</param>
    /// <param name="directory">Optional subdirectory path</param>
    /// <returns>
    /// Full physical path to the upload directory.
    /// Returns empty string if SiteConfigs.Environment is not initialized.
    /// </returns>
    /// <example>
    /// <code>
    /// var path = SystemDirectoryPaths.PrepareUploadDirectoryPath("user123", "profile");
    /// // Returns: "C:\app\wwwroot\contents\temp\user123/profile/"
    /// </code>
    /// </example>
    public static string PrepareUploadDirectoryPath(string? userId, string? directory)
    {
        if (SiteConfigs.Environment == null)
        {
            return string.Empty;
        }

        return Path.Combine(
            SiteConfigs.Environment.ContentRootPath,
            UploadDirectoryPath.Trim('/'),
            UtilityHelper.AdjustSlash(directory) ?? string.Empty
        );
    }

    /// <summary>
    /// Prepares a complete URL path for accessing uploaded content.
    /// </summary>
    /// <param name="userId">User identifier for user-specific content</param>
    /// <param name="directory">Optional subdirectory path</param>
    /// <returns>
    /// Full URL path to the uploaded content.
    /// </returns>
    public static string PrepareUploadUrlPath(string userId, string? directory)
    {
        return $"{ApplicationSettings.Domain.Backend?.TrimEnd('/')}/{UploadDirectoryUrl.Trim('/')}/{UtilityHelper.AdjustSlash(directory)?.Trim('/')}";
    }

    /// <summary>
    /// Prepares a directory path by combining base path with specified directory.
    /// </summary>
    /// <param name="path">Base directory path</param>
    /// <param name="directory">Target subdirectory</param>
    /// <param name="defaultDirectory">Fallback directory if target is not specified (default: "thumbs/")</param>
    /// <returns>
    /// Combined and properly formatted directory path.
    /// </returns>
    public static string PrepareDirectoryPath(string path, string directory, string defaultDirectory = "thumbs/")
    {
        var adjustedPath = UtilityHelper.AdjustSlash(path);
        var targetDirectory = !string.IsNullOrEmpty(directory)
            ? UtilityHelper.AdjustSlash(directory)
            : UtilityHelper.AdjustSlash(defaultDirectory);

        return adjustedPath + targetDirectory;
    }

    /// <summary>
    /// Prepares a local URL path by combining site URL with specified directory.
    /// </summary>
    /// <param name="directory">Target directory path</param>
    /// <param name="defaultDirectory">Fallback directory if target is not specified (default: "thumbs/")</param>
    /// <returns>
    /// Complete local URL path.
    /// </returns>
    public static string PrepareLocalUrl(string directory, string defaultDirectory = "thumbs/")
    {
        var baseUrl = ApplicationSettings.Domain.Backend?.TrimEnd('/') ?? string.Empty;
        var targetDirectory = !string.IsNullOrEmpty(directory)
            ? UtilityHelper.AdjustSlash(directory).Trim('/')
            : UtilityHelper.AdjustSlash(defaultDirectory).Trim('/');

        return $"{baseUrl}/{targetDirectory}";
    }

    #endregion
}