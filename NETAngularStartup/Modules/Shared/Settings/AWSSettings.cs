/*namespace DevCodeArchitect.Utilities;

/// <summary>
/// Represents configuration settings for Amazon Web Services (AWS) integration.
/// This class contains static properties that map to AWS-related settings in the application configuration.
/// </summary>
/// <remarks>
/// These settings are typically loaded from configuration files (e.g., appsettings.json)
/// during application startup and used throughout the application for AWS service integration.
/// </remarks>
public class AWSSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether AWS integration is enabled.
    /// </summary>
    /// <value>
    /// <c>true</c> if AWS services should be used; otherwise, <c>false</c>.
    /// Default is <c>false</c>.
    /// </value>
    public static bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the AWS region endpoint where services are hosted.
    /// </summary>
    /// <value>
    /// The AWS region identifier (e.g., "us-east-1", "eu-west-2").
    /// </value>
    /// <example>"us-east-1"</example>
    public static string? Region { get; set; }

    /// <summary>
    /// Gets or sets the name of the primary S3 bucket used for storage operations.
    /// </summary>
    /// <value>
    /// The S3 bucket name following AWS naming conventions.
    /// </value>
    public static string? Bucket { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the AWS CloudFront CDN distribution.
    /// </summary>
    /// <value>
    /// The CDN endpoint URL (e.g., "https://d123.cloudfront.net").
    /// Null if CDN is not configured.
    /// </value>
    public static string? CdnUrl { get; set; }
}*/