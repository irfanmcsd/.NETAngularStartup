/*namespace DevCodeArchitect.Utilities;

/// <summary>
/// Represents configuration settings for media handling in the application.
/// This class provides static access to various media-related configuration parameters.
/// </summary>
/// <remarks>
/// These settings are typically loaded from configuration files (e.g., appsettings.json)
/// during application startup and control how media files are processed and managed.
/// </remarks>
public class MediaSettings
{
    #region Media Processing Settings

    /// <summary>
    /// Gets or sets a value indicating whether image resizing should be performed via AWS Lambda.
    /// </summary>
    /// <value>
    /// <c>true</c> to use AWS Lambda for image resizing; otherwise, <c>false</c> to use local processing.
    /// </value>
    public static bool ResizeImageViaLambda { get; set; }

    /// <summary>
    /// Gets or sets the source directory where media files are initially uploaded.
    /// </summary>
    /// <value>
    /// The path to the source directory (e.g., "/uploads/original/").
    /// </value>
    public static string? SourceDirectory { get; set; }

    #endregion

    #region Photo Settings

    /// <summary>
    /// Gets or sets the allowed file extensions for photos.
    /// </summary>
    /// <value>
    /// A comma-separated list of extensions (e.g., ".jpg,.png,.gif").
    /// </value>
    public static string? PhotoExtensions { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed photo size.
    /// </summary>
    /// <value>
    /// The size specification (e.g., "5MB" or "1920x1080").
    /// </value>
    public static string? PhotoSize { get; set; }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets a subset of media settings as an anonymous object.
    /// </summary>
    /// <returns>
    /// An object containing the photo-related settings (extensions and size).
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            PhotoExtensions,
            PhotoSize
        };
    }

    #endregion
}*/