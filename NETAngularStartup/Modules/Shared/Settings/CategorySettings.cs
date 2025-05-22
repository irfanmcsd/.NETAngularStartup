namespace DevCodeArchitect.Utilities;

/// <summary>
/// Represents configuration settings for category-related media properties.
/// This class provides static access to category thumbnail configuration parameters.
/// </summary>
/// <remarks>
/// These settings control the dimensions of category thumbnails, default thumbnail images,
/// and AWS directory structures for category thumbnails.
/// </remarks>
public class CategorySettings
{
    #region Thumbnail Dimensions

    /// <summary>
    /// Gets or sets the width in pixels for category thumbnails.
    /// Typical values range from 100-500 pixels depending on display requirements.
    /// </summary>
    /// <example>300</example>
    public static int ThumbWidth { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels for category thumbnails.
    /// Should maintain aspect ratio consistent with ThumbWidth.
    /// </summary>
    /// <example>200</example>
    public static int ThumbHeight { get; set; }

    #endregion

    #region Default Thumbnail Settings

    /// <summary>
    /// Gets or sets the default thumbnail image path for categories without custom thumbnails.
    /// </summary>
    /// <value>
    /// Relative path to the default thumbnail image (e.g., "/images/default-category.png").
    /// Null indicates no default thumbnail is specified.
    /// </value>
    public static string? DefaultThumbnail { get; set; }

    #endregion

    #region AWS Storage Settings

    /// <summary>
    /// Gets or sets the AWS S3 directory name for storing category thumbnails.
    /// </summary>
    /// <value>
    /// AWS directory path (e.g., "categories/thumbs/").
    /// Should end with a forward slash.
    /// </value>
    public static string? AwsThumbDirname { get; set; }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the thumbnail dimension settings as an anonymous object.
    /// </summary>
    /// <returns>
    /// Object containing thumbnail dimensions:
    /// {
    ///     ThumbWidth,
    ///     ThumbHeight
    /// }
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            ThumbWidth,
            ThumbHeight
        };
    }

    #endregion
}