/*namespace DevCodeArchitect.Utilities
{
    /// <summary>
    /// Represents sensitive credentials and API keys loaded from appsettings.json.
    /// This class maps to the "Credentials" section in the configuration file.
    /// </summary>
    /// <remarks>
    /// WARNING: This class contains sensitive information. Proper security measures should be implemented:
    /// - Never commit actual credentials to source control
    /// - Use environment variables or secret managers in production
    /// - Implement proper access controls
    /// </remarks>
    public class SiteCredentials
    {
        #region Database Credentials

        /// <summary>
        /// Gets or sets the name of the database to connect to
        /// </summary>
        public static string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the database server hostname or IP address
        /// </summary>
        public static string DatabaseHost { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username for database authentication
        /// </summary>
        public static string DatabaseUser { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for database authentication
        /// </summary>
        public static string DatabasePassword { get; set; } = string.Empty;

        #endregion

        #region AWS Credentials

        /// <summary>
        /// Gets or sets the AWS access key ID for S3 and other AWS services
        /// </summary>
        public static string AwsAccessKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the AWS secret access key for S3 and other AWS services
        /// </summary>
        /// <remarks>
        /// Note: Property name contains a typo ("secretekey" instead of "secretkey")
        /// </remarks>
        public static string AwsSecretKey { get; set; } = string.Empty;

        #endregion

        #region Email Service Credentials

        /// <summary>
        /// Gets or sets whether Mandrill email service is enabled
        /// </summary>
        public static bool EnableMandrill { get; set; }

        /// <summary>
        /// Gets or sets the API key for Mandrill email service
        /// </summary>
        public static string MandrillKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether AWS Simple Email Service (SES) is enabled
        /// </summary>
        public static bool EnableSes { get; set; }

        /// <summary>
        /// Gets or sets the host endpoint for AWS SES
        /// </summary>
        public static string SesHost { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username for AWS SES SMTP authentication
        /// </summary>
        public static string SesUsername { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for AWS SES SMTP authentication
        /// </summary>
        public static string SesPassword { get; set; } = string.Empty;

        #endregion

        #region Security Credentials

        /// <summary>
        /// Gets or sets the private key used for JWT token generation and validation
        /// </summary>
        public static string JwtPrivateKey { get; set; } = string.Empty;

        #endregion

        #region Google OAuth Credentials

        /// <summary>
        /// Toggle on / off Google OAuth authentication
        /// </summary>
        public static bool EnableGoogleAuth { get; set; }

        /// <summary>
        /// Gets or sets Google OAuth - Client ID
        /// </summary>
        public static string GoogleClientID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Google OAuth - Client Secrete
        /// </summary>
        public static string GoogleClientSecret { get; set; } = string.Empty;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets a subset of credential settings as an anonymous object
        /// </summary>
        /// <returns>
        /// Object containing selected credential settings (currently empty)
        /// </returns>
        public static object GetSettings()
        {
            return new
            {
                GoogleClientID
                // Example of how to include specific credentials:
                // GoogleMapApiKey = GoogleMapApiKey,
                // StripeSiteKey = StripeSiteKey
            };
        }

        #endregion
    }
}*/