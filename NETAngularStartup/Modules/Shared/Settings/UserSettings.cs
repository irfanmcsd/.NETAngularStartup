namespace DevCodeArchitect.Utilities;

/// <summary>
/// Represents configuration settings for user-related media properties.
/// This class provides static access to user avatar configuration parameters.
/// </summary>
/// <remarks>
/// These settings control the dimensions of user avatars, default avatar images,
/// and AWS directory structures for avatar storage.
/// </remarks>
public class UserSettings
{
    #region Avatar Dimensions

    /// <summary>
    /// Gets or sets the width in pixels for user avatars.
    /// Typical values range from 100-300 pixels for profile pictures.
    /// </summary>
    /// <example>200</example>
    public static int AvatarWidth { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels for user avatars.
    /// Should maintain a square aspect ratio (1:1) for most applications.
    /// </summary>
    /// <example>200</example>
    public static int AvatarHeight { get; set; }

    #endregion

    #region Default Avatar Settings

    /// <summary>
    /// Gets or sets the default avatar image path for users without custom avatars.
    /// </summary>
    /// <value>
    /// Relative path to the default avatar image (e.g., "/images/default-avatar.png").
    /// Null indicates no default avatar is specified.
    /// </value>
    public static string? DefaultThumb { get; set; }

    #endregion

    #region AWS Storage Settings

    /// <summary>
    /// Gets or sets the AWS S3 directory name for storing user avatars.
    /// </summary>
    /// <value>
    /// AWS directory path (e.g., "users/avatars/").
    /// Should end with a trailing slash for proper path concatenation.
    /// </value>
    public static string? AwsAvatarDirname { get; set; }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the avatar dimension settings as an anonymous object.
    /// </summary>
    /// <returns>
    /// Object containing avatar dimensions:
    /// {
    ///     AvatarWidth,
    ///     AvatarHeight
    /// }
    /// </returns>
    public static object GetSettings()
    {
        return new
        {
            AvatarWidth,
            AvatarHeight
        };
    }

    #endregion

    #region Recommended Constants (for future implementation)

    /*
    public const int DefaultAvatarSize = 200;
    public const int MinimumAvatarSize = 50;
    public const int MaximumAvatarSize = 1024;
    public const string DefaultAvatarPath = "/images/default-avatar.png";
    */

    #endregion
}