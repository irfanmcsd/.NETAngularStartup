namespace DevCodeArchitect.Utilities;

public class MediaSettings
{
    public static string AllowedTypes { get; set; } = string.Empty;
    public static string MaxFileSize { get; set; } = string.Empty;

    public static MediaProcessingSettings ImageProcessing { get; set; } = new MediaProcessingSettings();

    public static MediaDefaultImages DefaultImages { get; set; } = new MediaDefaultImages();

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
            PhotoExtensions = AllowedTypes,
            PhotoSize = MaxFileSize
        };
    }

}

public class MediaProcessingSettings
{
    public int Quality { get; set; } = 0;
    public MediaProcessResolutionSettings Resolutions { get; set; } = new MediaProcessResolutionSettings();

}

public class MediaProcessResolutionSettings
{
    public string Thumbnail { get; set; } = string.Empty;
    public string Medium { get; set;} = string.Empty;
    public string Original { get; set; } = string.Empty;
}

public class MediaDefaultImages
{
    public string UserAvatar { get; set;} = string.Empty;
    public string BlogCover { get; set;} = string.Empty;
}
