namespace DevCodeArchitect.Utilities;

/// <summary>
/// Represents configuration settings for blog-related media and display properties.
/// This class provides static access to various blog-specific configuration parameters.
/// </summary>
/// <remarks>
/// These settings control the dimensions of blog images, default media files, 
/// and AWS directory structures for blog content.
/// </remarks>
public class BlogSettings
{
    #region Thumbnail Settings

    /// <summary>
    /// Gets or sets the width in pixels for blog thumbnails.
    /// Default value: 300 (typical thumbnail width)
    /// </summary>
    public static int ThumbWidth { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels for blog thumbnails.
    /// Default value: 200 (typical thumbnail height)
    /// </summary>
    public static int ThumbHeight { get; set; }

    /// <summary>
    /// Gets or sets the default thumbnail image path when none is specified.
    /// </summary>
    /// <value>
    /// Relative path to the default thumbnail image (e.g., "/images/default-thumb.png")
    /// </value>
    public static string? DefaultThumbnail { get; set; }

    /// <summary>
    /// Gets or sets the AWS directory name for storing thumbnail images.
    /// </summary>
    /// <value>
    /// AWS S3 directory path (e.g., "blogs/thumbs/")
    /// </value>
    public static string? AwsThumbDirname { get; set; }

    #endregion

    #region Banner/Cover Settings

    /// <summary>
    /// Gets or sets the width in pixels for blog banners/cover images.
    /// Default value: 1200 (typical banner width)
    /// </summary>
    public static int BannerWidth { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels for blog banners/cover images.
    /// Default value: 630 (typical banner height)
    /// </summary>
    public static int BannerHeight { get; set; }

    /// <summary>
    /// Gets or sets the default cover image path when none is specified.
    /// </summary>
    /// <value>
    /// Relative path to the default cover image (e.g., "/images/default-cover.jpg")
    /// </value>
    public static string? DefaultCover { get; set; }

    /// <summary>
    /// Gets or sets the AWS directory name for storing cover images.
    /// </summary>
    /// <value>
    /// AWS S3 directory path (e.g., "blogs/covers/")
    /// </value>
    public static string? AwsCoverDirname { get; set; }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the dimension settings as an anonymous object.
    /// </summary>
    /// <returns>
    /// Object containing thumbnail and banner dimensions:
    /// {
    ///     ThumbWidth,
    ///     ThumbHeight,
    ///     BannerWidth,
    ///     BannerHeight
    /// }
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            ThumbWidth,
            ThumbHeight,
            BannerWidth,
            BannerHeight
        };
    }

    #endregion
}