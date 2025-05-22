using DevCodeArchitect.Identity;
using DevCodeArchitect.Localize;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace DevCodeArchitect.Settings
{
    /// <summary>
    /// Centralized configuration class that holds all application settings and services
    /// that are injected via middleware or controllers. This serves as a shared access point
    /// for application-wide configurations and services.
    /// </summary>
    /// <remarks>
    /// This class uses static properties to provide global access to various services and settings.
    /// It follows the Ambient Context pattern, which can be useful but should be used judiciously.
    /// </remarks>
    public class SiteConfigs
    {
        #region Service Dependencies

        /// <summary>
        /// Gets or sets the memory cache instance for application-wide caching.
        /// </summary>
        /// <value>
        /// The <see cref="IMemoryCache"/> instance or null if not initialized.
        /// </value>
        public static IMemoryCache? Cache { get; set; }

        /// <summary>
        /// Gets or sets the UserManager for managing application users.
        /// </summary>
        /// <value>
        /// The <see cref="UserManager{TUser}"/> instance for <see cref="ApplicationUser"/> or null if not initialized.
        /// </value>
        public static UserManager<ApplicationUser>? UserManager { get; set; }

        /// <summary>
        /// Gets or sets the RoleManager for managing application roles.
        /// </summary>
        /// <value>
        /// The <see cref="RoleManager{TRole}"/> instance for <see cref="ApplicationRole"/> or null if not initialized.
        /// </value>
        public static RoleManager<ApplicationRole>? RoleManager { get; set; }

        /// <summary>
        /// Gets or sets the localizer for general resources.
        /// </summary>
        /// <value>
        /// The <see cref="IStringLocalizer{T}"/> instance for <see cref="GeneralResource"/> or null if not initialized.
        /// </value>
        public static IStringLocalizer<GeneralResource>? GeneralLocalizer { get; set; }

        /// <summary>
        /// Gets or sets the web host environment information.
        /// </summary>
        /// <value>
        /// The <see cref="IWebHostEnvironment"/> instance or null if not initialized.
        /// </value>
        public static IWebHostEnvironment? Environment { get; set; }

        /// <summary>
        /// Gets or sets the HTTP context accessor for accessing the current HTTP context.
        /// </summary>
        /// <value>
        /// The <see cref="IHttpContextAccessor"/> instance or null if not initialized.
        /// </value>
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }

        #endregion

        #region Application Settings

        /// <summary>
        /// Gets or sets the general application configuration
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.ApplicationSettings"/> instance or null if not initialized.
        /// </value>
        public static ApplicationSettings? ApplicationSettings { get; set; }

        
        /// <summary>
        /// Gets or sets the general jwt setting configuration
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.JWTSettings"/> instance or null if not initialized.
        /// </value>
        public static JWTSettings? JWTSettings { get; set; }


        /// <summary>
        /// Gets or sets the application email settings
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.EmailSettings"/> instance or null if not initialized.
        /// </value>
        public static EmailSettings? EmailSettings { get; set; }


        /// <summary>
        /// Gets or sets the aws configuration
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.AwsSettings"/> instance or null if not initialized.
        /// </value>
        public static AwsSettings? AwsSettings { get; set; }

        /// <summary>
        /// Gets or sets the general app secrets
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.AppSecrets"/> instance or null if not initialized.
        /// </value>
        public static AppSecrets? AppSecrets { get; set; }

        /// <summary>
        /// Gets or sets the media-related settings (e.g., file upload configurations).
        /// </summary>
        /// <value>
        /// The <see cref="Utilities.MediaSettings"/> instance or null if not initialized.
        /// </value>
        public static MediaSettings? MediaSettings { get; set; }

       

        /// <summary>
        /// Gets or sets the category-related settings.
        /// </summary>
        /// <value>
        /// The <see cref="CategorySettings"/> instance or null if not initialized.
        /// </value>
        public static CategorySettings? CategorySettings { get; set; }

        /// <summary>
        /// Gets or sets the blog-related settings.
        /// </summary>
        /// <value>
        /// The <see cref="BlogSettings"/> instance or null if not initialized.
        /// </value>
        public static BlogSettings? BlogSettings { get; set; }

        /// <summary>
        /// Gets or sets the user management settings.
        /// </summary>
        /// <value>
        /// The <see cref="UserSettings"/> instance or null if not initialized.
        /// </value>
        public static UserSettings? UserSettings { get; set; }

        #endregion
    }
}