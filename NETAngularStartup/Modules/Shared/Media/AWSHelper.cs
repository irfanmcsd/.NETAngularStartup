/*******************************************************************************
* ADVANCED MEDIA PROCESSING - CLOUD STORAGE MODULE
* 
* This module handles all media upload, processing, and cloud storage operations
* including AWS S3 integration and thumbnail generation.
* 
* Key Features:
* - Supports both AWS and local storage
* - Lambda-based thumbnail processing
* - Multi-format media handling
* - CDN integration
*******************************************************************************/

using DevCodeArchitect.DBContext;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides advanced media processing capabilities including cloud storage integration,
/// thumbnail generation, and media format conversion.
/// </summary>
public class MediaHelper
{
    #region Public Methods

    /// <summary>
    /// Uploads media files to either AWS S3 or local storage based on configuration
    /// </summary>
    /// <param name="context">Database context for tracking uploads</param>
    /// <param name="files">List of media files to process</param>
    /// <param name="supportedMedia">List of supported media formats</param>
    /// <param name="localDirectoryPath">Local storage directory path</param>
    /// <param name="bucket">AWS S3 bucket name</param>
    /// <param name="cdnUrl">CDN base URL</param>
    /// <param name="thumbsViaLambda">Whether to use AWS Lambda for thumbnail generation</param>
    /// <param name="domainId">Domain identifier for organization</param>
    /// <returns>List of processed media objects with URLs</returns>
    public static async Task<List<MediaObject>> UploadAsync(
        ApplicationDBContext context,
        List<MediaObject>? files,
        List<SupportedMedia> supportedMedia,
        string localDirectoryPath,
        string bucket,
        string cdnUrl,
        bool thumbsViaLambda,
        short domainId)
    {
        var mediaFiles = new List<MediaObject>();

        if (files == null) return mediaFiles;

        if (AwsSettings.Enabled &&
            !string.IsNullOrEmpty(bucket) &&
            HasValidAwsCredentials())
        {
            mediaFiles = await ProcessAwsUploads(context, files, supportedMedia,
                bucket, localDirectoryPath, cdnUrl, thumbsViaLambda, domainId);
        }
        else
        {
            mediaFiles = ProcessLocalFiles(files);
        }

        return mediaFiles;
    }

    #endregion

    #region Private Processing Methods

    private static bool HasValidAwsCredentials()
    {
        return !string.IsNullOrEmpty(AppSecrets.Aws.AccessKeyId) &&
               !string.IsNullOrEmpty(AppSecrets.Aws.SecretAccessKey);
    }

    private static List<MediaObject> ProcessLocalFiles(List<MediaObject> files)
    {
        return files.Select(file =>
        {
            file.Hosted = "local";
            return file;
        }).ToList();
    }

    private static async Task<List<MediaObject>> ProcessAwsUploads(
        ApplicationDBContext context,
        List<MediaObject> files,
        List<SupportedMedia> supportedMedia,
        string bucket,
        string localDirectoryPath,
        string cdnUrl,
        bool thumbsViaLambda,
        short domainId)
    {
        var results = new List<MediaObject>();

        foreach (var file in files)
        {
            if (IsAlreadyUploaded(file))
            {
                if (file.Deleted != true) results.Add(file);
                continue;
            }

            if (!string.IsNullOrEmpty(file.Key))
            {
                var processedFile = await ProcessMediaFile(
                    context, file, supportedMedia, bucket,
                    localDirectoryPath, cdnUrl, thumbsViaLambda, domainId);

                if (processedFile != null)
                {
                    results.Add(processedFile);
                }
            }
        }

        return results;
    }

    private static bool IsAlreadyUploaded(MediaObject file)
    {
        return (!string.IsNullOrEmpty(file.Url) && file.Url.StartsWith("http")) ||
               (!string.IsNullOrEmpty(file.Key) && file.Key.StartsWith("http"));
    }

    private static async Task<MediaObject?> ProcessMediaFile(
        ApplicationDBContext context,
        MediaObject file,
        List<SupportedMedia> supportedMedia,
        string bucket,
        string localDirectoryPath,
        string cdnUrl,
        bool thumbsViaLambda,
        short domainId)
    {
        var processedFile = await UploadFileAsync(
            context, file, supportedMedia, bucket,
            localDirectoryPath, thumbsViaLambda, domainId);

        return processedFile != null ?
            PrepareUrl(processedFile, cdnUrl) : null;
    }

    /// <summary>
    /// Core uploader function that handles file processing and upload
    /// </summary>
    private static async Task<MediaObject?> UploadFileAsync(
        ApplicationDBContext context,
        MediaObject file,
        List<SupportedMedia> supportedMedia,
        string bucket,
        string dirPath,
        bool thumbsViaLambda,
        short domainId)
    {
        if (file == null || supportedMedia == null ||
            supportedMedia.Count == 0 ||
            string.IsNullOrEmpty(file.Key) ||
            string.IsNullOrEmpty(bucket))
        {
            return null;
        }

        var fileObject = new MediaObject
        {
            Key = UtilityHelper.ReplaceSpaceWithHyphen(file.Key),
            Files = new List<MediaFile>(),
            IsDefault = file.IsDefault,
            Hosted = "aws",
            Bucket = bucket
        };

        return thumbsViaLambda ?
            await ProcessLambdaMedia(context, file, fileObject, supportedMedia, bucket, dirPath, domainId) :
            await ProcessWebsiteMedia(context, file, fileObject, supportedMedia, bucket, dirPath, domainId);
    }

    private static async Task<MediaObject?> ProcessLambdaMedia(
        ApplicationDBContext context,
        MediaObject file,
        MediaObject fileObject,
        List<SupportedMedia> supportedMedia,
        string bucket,
        string dirPath,
        short domainId)
    {
        foreach (var format in supportedMedia)
        {
            if (format.IsOriginal != true) continue;

            var filePath = BuildLocalFilePath(dirPath, format.Directory, file.Key);
            if (!File.Exists(filePath)) return null;

            var cloudFilename = BuildCloudFilename(file.Key, format.CloudDirectory, domainId);
            await AmazonUtil.UploadFileToS3Async(context, string.Empty, cloudFilename, bucket, filePath);

            AddMediaFormat(fileObject, format, domainId);
        }

        return fileObject;
    }

    private static async Task<MediaObject?> ProcessWebsiteMedia(
        ApplicationDBContext context,
        MediaObject file,
        MediaObject fileObject,
        List<SupportedMedia> supportedMedia,
        string bucket,
        string dirPath,
        short domainId)
    {
        foreach (var format in supportedMedia)
        {
            var filePath = BuildLocalFilePath(dirPath, format.Directory, file.Key);
            if (!File.Exists(filePath)) return null;

            var cloudFilename = BuildCloudFilename(file.Key, format.CloudDirectory, domainId);
            await AmazonUtil.UploadFileToS3Async(context, string.Empty, cloudFilename, bucket, filePath);

            AddMediaFormat(fileObject, format, domainId);
        }

        return fileObject;
    }

    private static string BuildLocalFilePath(string basePath, string? directory, string filename)
    {
        var path = UtilityHelper.AdjustSlash(basePath);
        if (!string.IsNullOrEmpty(directory))
        {
            path += UtilityHelper.AdjustSlash(directory);
        }
        return path + filename;
    }

    private static string BuildCloudFilename(string filename, string? cloudDirectory, short domainId)
    {
        var cloudFilename = UtilityHelper.ReplaceSpaceWithHyphen(filename);

        if (!string.IsNullOrEmpty(cloudDirectory))
        {
            cloudFilename = domainId > 0 ?
                $"{UtilityHelper.PrepareDirName(cloudDirectory)}{domainId}/{cloudFilename}" :
                $"{UtilityHelper.PrepareDirName(cloudDirectory)}{cloudFilename}";
        }

        return cloudFilename;
    }

    private static void AddMediaFormat(MediaObject fileObject, SupportedMedia format, short domainId)
    {
        var directory = UtilityHelper.AdjustSlash(format.CloudDirectory);
        if (domainId > 0)
        {
            directory += $"{domainId}/";
        }

        fileObject.Files?.Add(new MediaFile
        {
            Attributes = new MediaAttributes
            {
                Format = format.Format,
                Width = format.Width,
                Height = format.Height
            },
            IsDefault = format.IsDefault,
            IsOriginal = format.IsOriginal,
            Directory = directory
        });
    }

    /// <summary>
    /// Generates public/private URL for media files using CDN if available
    /// </summary>
    public static MediaObject? PrepareUrl(MediaObject file, string? cdnUrl)
    {
        if (!string.IsNullOrEmpty(cdnUrl))
        {
            file.Url = UtilityHelper.AdjustSlash(cdnUrl);
        }
        return file;
    }

    #endregion
}

#region Supporting Classes

/// <summary>
/// Represents a supported media format configuration
/// </summary>
public class SupportedMedia
{
    public string? Format { get; set; }  // File format (e.g., ".webp", ".jpg")
    public int? Width { get; set; }      // Target width in pixels
    public int? Height { get; set; }     // Target height in pixels (null for auto)
    public string? Directory { get; set; }       // Local storage directory
    public string? CloudDirectory { get; set; }  // Cloud storage directory
    public bool? IsOriginal { get; set; }        // Marks original/origin file
    public bool? IsDefault { get; set; }         // Marks default format
}

/// <summary>
/// Represents a media object with all associated files and metadata
/// </summary>
public class MediaObject
{
    public string? Key { get; set; }         // Unique identifier/filename
    public string? Iframe { get; set; }      // HTML iframe embed code
    public string? YoutubeId { get; set; }   // YouTube video ID
    public string? Url { get; set; }         // Public access URL
    public bool? IsDefault { get; set; }     // Default selection flag
    public bool? Deleted { get; set; }       // Deletion status flag
    public string? Hosted { get; set; }      // Hosting location ("aws", "local")
    public string? Bucket { get; set; }      // AWS bucket name
    public List<MediaFile>? Files { get; set; }  // Associated media files
    public List<MediaThumbnail>? Thumbnails { get; set; }  // Thumbnail versions
}

/// <summary>
/// Represents a media thumbnail version
/// </summary>
public class MediaThumbnail
{
    public string? Key { get; set; }         // Thumbnail filename
    public string? Url { get; set; }         // Thumbnail URL
    public List<MediaFile>? Files { get; set; }  // Thumbnail files
}

/// <summary>
/// Represents a specific media file version with attributes
/// </summary>
public class MediaFile
{
    public MediaAttributes? Attributes { get; set; }  // File attributes
    public string? Directory { get; set; }           // Storage directory
    public bool? IsDefault { get; set; }             // Default version flag
    public bool? IsOriginal { get; set; }            // Original file flag
}

/// <summary>
/// Contains detailed attributes of a media file
/// </summary>
public class MediaAttributes
{
    public string? Format { get; set; }      // File format extension
    public int? Width { get; set; }         // Width in pixels
    public int? Height { get; set; }        // Height in pixels
    public string? Duration { get; set; }    // Duration (HH:MM:SS format)
    public int? DurationSec { get; set; }   // Duration in seconds
}

/// <summary>
/// Simple file entity for basic file operations
/// </summary>
public class FileEntity
{
    public string? Filename { get; set; }    // Original filename
    public string? ImageUrl { get; set; }    // Image display URL
    public string? Title { get; set; }       // Display title
    public bool? Selected { get; set; }      // Selection status
}

#endregion

/*
* This file is subject to the terms and conditions defined in
* file 'LICENSE.md', which is part of this source code package.
* Copyright 2007 - 2024 MediaSoftPro
* For more information email at support@mediasoftpro.com
*/