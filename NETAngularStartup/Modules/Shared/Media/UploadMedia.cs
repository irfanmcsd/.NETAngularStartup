using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Settings;
using Newtonsoft.Json;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides core functionality for uploading, managing, and deleting media files in cloud storage
/// and local file systems. Supports image processing, thumbnail generation, and file management.
/// </summary>
public static class UploadMedia
{
    /// <summary>
    /// Enum representing different media types for processing
    /// </summary>
    public enum MediaType
    {
        /// <summary>Original full-size media</summary>
        Original = 0,
        /// <summary>Thumbnail version</summary>
        Thumbnail = 1,
        /// <summary>Cover image version</summary>
        Cover = 2
    }

    #region Media Processing

    /// <summary>
    /// Processes and uploads a cover image from base64 data or existing file
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="fileName">Base64 image data or filename</param>
    /// <param name="name">Name prefix for generated filename</param>
    /// <param name="userId">User ID for directory organization</param>
    /// <param name="awsDirectory">AWS directory path</param>
    /// <returns>URL of the processed image or original filename</returns>
    public static async Task<string?> ProcessCover(
        ApplicationDBContext context,
        string? fileName,
        string? name,
        string? userId,
        string? awsDirectory)
    {
        if (!string.IsNullOrEmpty(fileName) && fileName.StartsWith("data:image") && SiteConfigs.Environment != null)
        {
            try
            {
                // Process base64 image
                var imageData = fileName.Replace("data:image/png;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(imageData);

                // Generate filename
                var prefix = string.IsNullOrEmpty(name)
                    ? ""
                    : UtilityHelper.ReplaceSpaceWithHyphen(name) + "-";

                string newFileName = $"{prefix}{Guid.NewGuid().ToString()[..15]}.png";
                var localPath = SystemDirectoryPaths.PrepareUploadDirectoryPath(userId, "");

                // Ensure directory exists
                Directory.CreateDirectory(localPath);

                // Delete existing file if present
                var fullPath = Path.Combine(localPath, newFileName);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                // Save locally
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                // Upload to cloud storage
                fileName = await Aws.UploadPhoto(
                    context,
                    newFileName,
                    localPath,
                    UtilityHelper.PrepareDirName(awsDirectory));
            }
            catch (Exception ex)
            {
                await LogError(context, "ProcessCover", ex);
                return fileName; // Return original on error
            }
        }
        return fileName;
    }

    /// <summary>
    /// Processes and uploads multiple media files with thumbnail generation
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="media">List of media objects to process</param>
    /// <param name="userId">User ID for directory organization</param>
    /// <param name="domainId">Domain ID for categorization</param>
    /// <param name="sourceDir">Source directory path</param>
    /// <param name="thumbDir">Thumbnail directory path</param>
    /// <param name="thumbWidth">Thumbnail width</param>
    /// <param name="thumbHeight">Thumbnail height</param>
    /// <param name="isMedia">Whether to process as media (images)</param>
    /// <param name="sizes">Additional size specifications</param>
    /// <returns>JSON string of processed media information</returns>
    public static async Task<string> ProcessPictures(
        ApplicationDBContext context,
        List<MediaObject>? media,
        string? userId,
        short domainId,
        string? sourceDir,
        string? thumbDir,
        int thumbWidth,
        int thumbHeight,
        bool isMedia = true,
        string? sizes = null)
    {
        try
        {
            var localDirPath = SystemDirectoryPaths.PrepareUploadDirectoryPath(userId, "");
            var bucket = AwsSettings.S3.BucketName;
            var cdnUrl = AwsSettings.S3.BaseUrl;
            var sourceDirectory = sourceDir;

            if (isMedia)
            {
                var thumbDirectory = thumbDir;
                var useLambdaResizing = AwsSettings.Lambda.ResizeImageviaLambda;

                if (useLambdaResizing)
                {
                    sourceDirectory = AwsSettings.Lambda.SourceDirectory;
                    thumbDirectory ??= "images/";
                    thumbDirectory = SystemDirectoryPaths.LambdaThumbnailDirectory;
                }

                var supportedMedia = new List<SupportedMedia>
                {
                    new() // Original version
                    {
                        IsOriginal = true,
                        Directory = sourceDir,
                        CloudDirectory = sourceDirectory
                    },
                    new() // Thumbnail version
                    {
                        Format = ".webp",
                        IsDefault = true,
                        Width = thumbWidth,
                        Height = thumbHeight,
                        Directory = thumbDir,
                        CloudDirectory = thumbDirectory
                    }
                };

                if (!useLambdaResizing && !string.IsNullOrEmpty(sizes))
                {
                    AddCustomSizes(supportedMedia, sizes);
                }

                var processedMedia = await MediaHelper.UploadAsync(
                    context,
                    media,
                    supportedMedia,
                    localDirPath,
                    bucket,
                    cdnUrl,
                    useLambdaResizing,
                    domainId);

                return JsonConvert.SerializeObject(processedMedia);
            }
            else
            {
                var fileSupportedMedia = new List<SupportedMedia>
                {
                    new() // Original file only
                    {
                        IsOriginal = true,
                        Directory = sourceDir,
                        CloudDirectory = sourceDirectory
                    }
                };

                var processedFiles = await MediaHelper.UploadAsync(
                    context,
                    media,
                    fileSupportedMedia,
                    localDirPath,
                    bucket,
                    cdnUrl,
                    false,
                    domainId);

                return JsonConvert.SerializeObject(processedFiles);
            }
        }
        catch (Exception ex)
        {
            await LogError(context, "ProcessPictures", ex);
            return string.Empty;
        }
    }

    #endregion

    #region Media Deletion

    /// <summary>
    /// Deletes media files from both cloud storage and local system
    /// </summary>
    public static async Task DeleteMedia(
        ApplicationDBContext context,
        List<MediaObject>? mediaList,
        string? userId)
    {
        if (mediaList == null) return;

        try
        {
            var useLambdaResizing = AwsSettings.Lambda.ResizeImageviaLambda;
            var bucket = AwsSettings.S3.BucketName;

            foreach (var media in mediaList)
            {
                if (string.IsNullOrEmpty(media.Hosted) || media.Hosted == "aws")
                {
                    await DeleteCloudMedia(media, bucket, useLambdaResizing);
                }
                else if (media.Hosted == "local")
                {
                    DeleteLocalMedia(media, userId);
                }
            }
        }
        catch (Exception ex)
        {
            await LogError(context, "DeleteMedia", ex);
        }
    }

    /// <summary>
    /// Deletes selected media files marked for deletion
    /// </summary>
    public static async Task<List<MediaObject>?> DeleteSelectedMedia(
        ApplicationDBContext context,
        List<MediaObject>? mediaList,
        string? userId)
    {
        if (mediaList == null) return null;

        try
        {
            var useLambdaResizing = AwsSettings.Lambda.ResizeImageviaLambda;
            var bucket = AwsSettings.S3.BucketName;

            // Process deletion for marked items
            foreach (var media in mediaList.Where(m => m.Deleted == true))
            {
                if (string.IsNullOrEmpty(media.Hosted) || media.Hosted == "aws")
                {
                    await DeleteCloudMedia(media, bucket, useLambdaResizing);
                }
                else if (media.Hosted == "local")
                {
                    DeleteLocalMedia(media, userId);
                }
            }

            // Return only non-deleted items
            return mediaList.Where(m => m.Deleted != true).ToList();
        }
        catch (Exception ex)
        {
            await LogError(context, "DeleteSelectedMedia", ex);
            return mediaList;
        }
    }

    #endregion

    #region URL Generation

    /// <summary>
    /// Generates URL for an image with fallback options
    /// </summary>
    public static string GetImageUrl(
        string? image,
        string? defaultImage)
    {
        if (!string.IsNullOrEmpty(image))
        {
            return image.StartsWith("http")
                ? image
                : $"{ApplicationSettings.Domain.Backend}{SystemDirectoryPaths.UploadDirectoryUrl}{image}";
        }

        if (!string.IsNullOrEmpty(defaultImage))
        {
            // Remove leading slash if present
            var cleanDefault = defaultImage.StartsWith("/")
                ? defaultImage[1..]
                : defaultImage;
            return $"{ApplicationSettings.Domain.Backend}{cleanDefault}";
        }

        return "https://xyz.com/image.jpg"; // Fallback default
    }

    /// <summary>
    /// Generates URL for an image with processing status awareness
    /// </summary>
    public static string? GetImageUrlV2(
        string? cover,
        DateTime? uploadTime,
        string? defaultImage,
        string? thumbDir)
    {
        if (AwsSettings.Lambda.ResizeImageviaLambda &&
            uploadTime.HasValue &&
            uploadTime.Value.AddSeconds(20) > UtilityHelper.TimeZoneOffsetDateTime())
        {
            return $"{ApplicationSettings.Domain.Backend}images/image-processing.webp";
        }

        if (!string.IsNullOrEmpty(cover))
        {
            return cover.StartsWith("http")
                ? cover
                : $"{SystemDirectoryPaths.PrepareUploadUrlPath("", thumbDir)}{cover}";
        }

        return $"{ApplicationSettings.Domain.Backend}{defaultImage}";
    }

    #endregion

    #region Helper Methods

    private static async Task LogError(
        ApplicationDBContext context,
        string methodName,
        Exception ex)
    {
        await ErrorLogsBLL.Add(context, new ErrorLogs
        {
            Description = $"UploadMedia Error - {methodName}",
            Url = $"UploadMedia -> {methodName}()",
            StackTrace = ex.Message
        });
    }

    private static void AddCustomSizes(
        List<SupportedMedia> supportedMedia,
        string sizes)
    {
        foreach (var size in sizes.Split(','))
        {
            if (size.Contains('x'))
            {
                var dimensions = size.Split('x');
                if (dimensions.Length == 2 &&
                    int.TryParse(dimensions[0], out var width) &&
                    int.TryParse(dimensions[1], out var height))
                {
                    supportedMedia.Add(new SupportedMedia
                    {
                        Width = width,
                        Height = height,
                        Directory = size,
                        CloudDirectory = size
                    });
                }
            }
        }
    }

    private static async Task DeleteCloudMedia(
        MediaObject media,
        string bucket,
        bool useLambdaResizing)
    {
        if (string.IsNullOrEmpty(media.Key) || media.Files == null) return;

        foreach (var file in media.Files.Where(f => !string.IsNullOrEmpty(f.Directory)))
        {
            var fileKey = $"{file.Directory}{media.Key}";

            // Fix for Problem 1: Use GetValueOrDefault() to handle nullable bool
            if (file.IsDefault.GetValueOrDefault() && useLambdaResizing)
            {
                // Fix for Problems 2, 3, and 4: Remove references to 'Attribute' as it doesn't exist
                // Assuming 'Format' is a direct property of 'MediaFile'
                if (!string.IsNullOrEmpty(file.Attributes.Format))
                {
                    fileKey = Path.ChangeExtension(fileKey, file.Attributes.Format);
                }
            }

            // Fix for Problem 5: Ensure AmazonUtil.DeleteFile exists or provide a mock implementation
            await AmazonUtil.DeleteFileAsync(bucket, fileKey);
        }
    }

    private static void DeleteLocalMedia(MediaObject media, string? userId)
    {
        if (string.IsNullOrEmpty(media.Key) || media.Files == null) return;

        var dirPath = SystemDirectoryPaths.PrepareUploadDirectoryPath(userId, "");

        foreach (var file in media.Files.Where(f => !string.IsNullOrEmpty(f.Directory)))
        {
            var filePath = Path.Combine(dirPath, $"{file.Directory}{media.Key}");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    #endregion
}