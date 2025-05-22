/*namespace DevCodeArchitect.Utilities
{
    /// <summary>
    /// Represents application configuration settings mapped from appsettings.json's SiteSettings section.
    /// This class provides static access to various site-wide configuration parameters.
    /// </summary>
    /// <remarks>
    /// All properties in this class correspond to configuration values that should be defined
    /// in the application's configuration file (appsettings.json) under the "SiteSettings" section.
    /// </remarks>
    public class SiteConfigurations
    {
        #region General Site Settings
        /// <summary>
        /// Gets or sets the base URL of the website.
        /// </summary>
        public static string AppTitle { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the base URL of the website.
        /// </summary>
        public static string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default page size for paginated results.
        /// </summary>
        public static int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the default currency symbol or code used throughout the application.
        /// </summary>
        public static string? Currency { get; set; }

        /// <summary>
        /// Gets or sets the caption/title displayed on web pages.
        /// </summary>
        public static string? PageCaption { get; set; }

        /// <summary>
        /// Gets or sets the timezone offset from UTC (e.g., "+05:30").
        /// </summary>
        public static string TimezoneOffset { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the author/meta author for the website.
        /// </summary>
        public static string? WebsiteAuthor { get; set; }

        /// <summary>
        /// Gets or sets the name of the cookie used for list preferences.
        /// </summary>
        public static string? ListCookieName { get; set; }

        #endregion

        #region JWT Authentication Settings

        /// <summary>
        /// Gets or sets the valid issuer for JWT tokens.
        /// </summary>
        public static string JwtValidIssuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the valid audience for JWT tokens.
        /// Note: Property name contains a typo ("audiance" instead of "audience").
        /// </summary>
        public static string JwtValidAudiance { get; set; } = string.Empty;

        #endregion

        #region Email Settings

        /// <summary>
        /// Gets or sets whether email functionality is enabled.
        /// </summary>
        public static bool EnableEmail { get; set; }
                
        /// <summary>
        /// Gets or sets the support email address used for sending emails.
        /// </summary>
        public static string? SupportEmail { get; set; }

        /// <summary>
        /// Gets or sets the SMTP email address used for sending emails.
        /// </summary>
        public static string? SmtpEmail { get; set; }

        /// <summary>
        /// Gets or sets the display name used when sending emails.
        /// </summary>
        public static string? SmtpDisplayName { get; set; }

        #endregion

        #region Pagination & Content Settings

        /// <summary>
        /// Gets or sets the number of pagination links to display.
        /// </summary>
        public static int PaginationLinks { get; set; }

        /// <summary>
        /// Gets or sets the content approval setting (0 = disabled, 1 = enabled, etc.).
        /// </summary>
        public static int ContentApproval { get; set; }

        #endregion

        #region Caching & Performance Settings

        /// <summary>
        /// Gets or sets the duration (in minutes) for cached items.
        /// </summary>
        public static int CacheDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) before content is archived.
        /// </summary>
        public static int ArchiveDuration { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of pages to cache.
        /// </summary>
        public static int MaxCachePages { get; set; }

        /// <summary>
        /// Gets or sets whether search queries should be stored.
        /// </summary>
        public static bool StoreSearches { get; set; }

        #endregion

        #region SEO & URL Settings

        /// <summary>
        /// Gets or sets the maximum number of characters allowed in URLs.
        /// </summary>
        public static int MaximumUrlCharacters { get; set; }

        /// <summary>
        /// Gets or sets whether web indexing is enabled for search engines.
        /// </summary>
        public static bool WebIndexing { get; set; }

        #endregion

        #region Development Settings

        /// <summary>
        /// Gets or sets whether the application is in development mode.
        /// </summary>
        public static bool DevelopmentMode { get; set; }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Returns a subset of settings as an anonymous object.
        /// </summary>
        /// <returns>
        /// An anonymous object containing PageSize and Currency settings.
        /// </returns>
        public static object GetSettings()
        {
            return new
            {
                PageSize,
                Currency
            };
        }

        #endregion
    }
}*/