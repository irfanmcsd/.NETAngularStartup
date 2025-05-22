namespace DevCodeArchitect.Utilities;

public class AwsSettings
{
    public static bool Enabled { get; set; }
    public static string Region { get; set; } = string.Empty;

    public static AWSS3Bucket S3 { get; set; } = new AWSS3Bucket();

    public static AWSLambdaSettings Lambda { get; set; } = new AWSLambdaSettings();
}

public class AWSS3Bucket
{
    public string BucketName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public AWSBucketDirectories Directories { get; set; } = new AWSBucketDirectories();
}

public class AWSBucketDirectories
{
    public string Media { get; set;} = string.Empty;
    public string Avatars { get; set; } = string.Empty;
    public string Blog { get; set; } = string.Empty;
}

public class AWSLambdaSettings
{
    public bool ResizeImageviaLambda { get; set; } = false;
    public string SourceDirectory { get; set; } = string.Empty;
}