using Amazon.S3;
using Amazon.S3.Model;
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides utility methods for interacting with Amazon S3 storage service.
/// Handles file uploads, downloads, deletions, and URL generation with proper access control.
/// </summary>
/// <remarks>
/// This class encapsulates all S3 operations and provides both synchronous and asynchronous methods.
/// It requires AWS credentials (access key and secret key) to be configured.
/// </remarks>
public class AmazonUtil
{
    #region URL Generation

    /// <summary>
    /// Generates a pre-signed URL for temporary access to an S3 object
    /// </summary>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="folder">Folder path within the bucket (optional)</param>
    /// <param name="fileName">Name of the file to access</param>
    /// <param name="expires">Minutes until URL expiration (default: 10)</param>
    /// <returns>Pre-signed URL string</returns>
    public static string GetPreSignedUrl(string bucketName, string folder, string fileName, int expires = 10)
    {
        using var client = GetS3Client();
        var objectKey = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            Expires = UtilityHelper.TimeZoneOffsetDateTime().AddMinutes(expires)
        };

        return client.GetPreSignedURL(request);
    }

    #endregion

    #region File Deletion

    /// <summary>
    /// Deletes multiple files from S3 that match the given prefix
    /// </summary>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="prefix">Prefix to filter files for deletion</param>
    /// <param name="delimiter">Directory delimiter (default: "/")</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task DeleteFilesByPrefixAsync(string bucketName, string prefix, string delimiter = "/")
    {
        var fileList = GetFileList(bucketName, prefix, delimiter);
        foreach (var item in fileList)
        {
            await DeleteFileAsync(bucketName, item.Key);
        }
    }

    /// <summary>
    /// Deletes a single file from S3 storage
    /// </summary>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="fileName">Key of the file to delete</param>
    /// <returns>HTTP status code of the deletion operation</returns>
    public static async Task<string?> DeleteFileAsync(string? bucketName, string fileName)
    {
        if (string.IsNullOrEmpty(bucketName)) return null;

        using var client = GetS3Client();
        var response = await client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = fileName
        });

        return response.HttpStatusCode.ToString();
    }

    #endregion

    #region File Listing

    /// <summary>
    /// Retrieves a list of S3 objects matching the given prefix
    /// </summary>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="prefix">Prefix to filter objects</param>
    /// <param name="delimiter">Directory delimiter (default: "/")</param>
    /// <returns>List of S3 objects matching the criteria</returns>
    /// <exception cref="AmazonS3Exception">Thrown when S3 access fails</exception>
    public static List<S3Object> GetFileList(string bucketName, string prefix, string delimiter = "/")
    {
        using var client = GetS3Client();
        var request = new ListObjectsRequest
        {
            BucketName = bucketName,
            Prefix = prefix,
            Delimiter = delimiter
        };

        var response = client.ListObjectsAsync(request).Result;
        return response.S3Objects.ToList();
    }

    #endregion

    #region File Download

    /// <summary>
    /// Downloads a file from S3 into a MemoryStream
    /// </summary>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="folder">Folder path within the bucket (optional)</param>
    /// <param name="fileName">Name of the file to download</param>
    /// <returns>MemoryStream containing the file data</returns>
    /// <exception cref="AmazonS3Exception">Thrown when download fails</exception>
    public static MemoryStream DownloadFile(string bucketName, string folder, string fileName)
    {
        using var client = GetS3Client();
        var objectKey = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";
        var memoryStream = new MemoryStream();

        using var response = client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey
        }).Result;

        using var bufferedStream = new BufferedStream(response.ResponseStream);
        bufferedStream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    #endregion

    #region File Upload

    /// <summary>
    /// Uploads a file from a stream to S3 storage
    /// </summary>
    /// <param name="folder">Target folder path in S3 (optional)</param>
    /// <param name="fileName">Name to give the uploaded file</param>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="stream">Stream containing file data</param>
    /// <returns>HTTP status code of the upload operation</returns>
    public static string UploadFile(string folder, string fileName, string bucketName, Stream stream)
    {
        using var client = GetS3Client();
        var objectKey = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

        var response = client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            InputStream = stream,
            CannedACL = S3CannedACL.PublicRead
        }).Result;

        return response.HttpStatusCode.ToString();
    }

    /// <summary>
    /// Uploads a local file to S3 storage (synchronous)
    /// </summary>
    /// <param name="folder">Target folder path in S3 (optional)</param>
    /// <param name="fileName">Name to give the uploaded file</param>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="filePath">Local path of the file to upload</param>
    /// <returns>HTTP status code or "OK" if successful</returns>
    /// <exception cref="AmazonS3Exception">Thrown when upload fails</exception>
    public static string UploadFileToS3(string folder, string fileName, string bucketName, string filePath)
    {
        if (!File.Exists(filePath)) return "OK";

        using var client = GetS3Client();
        var objectKey = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

        var response = client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            FilePath = filePath,
            CannedACL = S3CannedACL.PublicRead
        }).Result;

        File.Delete(filePath);
        return string.IsNullOrEmpty(response.HttpStatusCode.ToString()) ? "OK" : response.HttpStatusCode.ToString();
    }

    /// <summary>
    /// Uploads a local file to S3 storage (asynchronous)
    /// </summary>
    /// <param name="context">Database context for error logging</param>
    /// <param name="folder">Target folder path in S3 (optional)</param>
    /// <param name="fileName">Name to give the uploaded file</param>
    /// <param name="bucketName">Name of the S3 bucket</param>
    /// <param name="filePath">Local path of the file to upload</param>
    /// <returns>"OK" if successful, "none" if failed, logs errors to database</returns>
    public static async Task<string> UploadFileToS3Async(
        ApplicationDBContext context,
        string folder,
        string fileName,
        string? bucketName,
        string filePath)
    {
        if (!File.Exists(filePath) || string.IsNullOrEmpty(bucketName))
            return "OK";

        try
        {
            using var client = GetS3Client();
            var objectKey = string.IsNullOrEmpty(folder) ? fileName.Trim() : $"{folder.Trim()}/{fileName.Trim()}";

            var response = await client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName.Trim(),
                Key = objectKey,
                FilePath = filePath,
                CannedACL = S3CannedACL.PublicRead
            });

            File.Delete(filePath);
            return string.IsNullOrEmpty(response.HttpStatusCode.ToString()) ? "OK" : response.HttpStatusCode.ToString();
        }
        catch (AmazonS3Exception s3Exception)
        {
            await ErrorLogsBLL.Add(context, new ErrorLogs
            {
                Description = "S3 Upload Error",
                Url = $"{fileName} _ {filePath}",
                StackTrace = s3Exception.Message
            });

            return "none";
        }
    }

    #endregion

    #region Client Management

    /// <summary>
    /// Creates and returns a configured AmazonS3Client instance
    /// </summary>
    /// <returns>Configured S3 client with credentials from application settings</returns>
    private static AmazonS3Client GetS3Client()
    {
        Amazon.AWSConfigs.AWSRegion = AwsSettings.Region;
        return new AmazonS3Client(AppSecrets.Aws.AccessKeyId, AppSecrets.Aws.SecretAccessKey);
    }

    #endregion
}