using DevCodeArchitect.Localize;
using DevCodeArchitect.Settings;
using DevCodeArchitect.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DevCodeArchitect.Middlewares
{
    /// <summary>
    /// Middleware for initializing and distributing application configuration settings
    /// throughout the application via the SiteConfigs static class.
    /// </summary>
    /// <remarks>
    /// This middleware runs early in the request pipeline to make configuration and services
    /// available globally via static access. Use judiciously as static dependencies can make
    /// testing harder.
    /// </remarks>
    public class ConfigMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the ConfigMiddleware
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public ConfigMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Middleware invocation method that configures shared application resources
        /// </summary>
        /// <param name="httpContext">The current HTTP context</param>
        /// <param name="memoryCache">The memory cache service</param>
        /// <param name="generalLocalizer">Localization service for general resources</param>
        /// <param name="environment">Hosting environment information</param>
        /// <param name="httpContextAccessor">HTTP context accessor service</param>
        /// <param name="siteConfigurations">Site configuration options</param>
        /// <param name="siteCredentials">Site credential options</param>
        /// <param name="mediaSettings">Media settings options</param>
        /// <param name="awsSettings">AWS settings options</param>
        /// <param name="categorySettings">Category settings options</param>
        /// <param name="blogSettings">Blog settings options</param>
        /// <param name="userSettings">User settings options</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public Task InvokeAsync(
            HttpContext httpContext,
            IMemoryCache memoryCache,
            IStringLocalizer<GeneralResource> generalLocalizer,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSecrets> appSecrets,
            IOptions<ApplicationSettings> applicationSettings,
            IOptions<EmailSettings> emailSettings,
            IOptions<JWTSettings> jwtSettings,
            IOptions<MediaSettings> mediaSettings,
            IOptions<AwsSettings> awsSettings,
            IOptions<CategorySettings> categorySettings,
            IOptions<BlogSettings> blogSettings,
            IOptions<UserSettings> userSettings)
        {
            // Validate required services
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            if (memoryCache == null) throw new ArgumentNullException(nameof(memoryCache));
            if (generalLocalizer == null) throw new ArgumentNullException(nameof(generalLocalizer));
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (httpContextAccessor == null) throw new ArgumentNullException(nameof(httpContextAccessor));

            // Initialize shared service references
            InitializeSharedServices(
                memoryCache,
                generalLocalizer,
                environment,
                httpContextAccessor);

            // Initialize shared configuration settings
            InitializeSharedSettings(
                jwtSettings.Value,
                appSecrets.Value,
                applicationSettings.Value,
                emailSettings.Value,
                mediaSettings.Value,
                awsSettings.Value,
                categorySettings.Value,
                blogSettings.Value,
                userSettings.Value);

            return _next(httpContext);
        }

        /// <summary>
        /// Initializes shared service instances in SiteConfigs
        /// </summary>
        private static void InitializeSharedServices(
            IMemoryCache memoryCache,
            IStringLocalizer<GeneralResource> generalLocalizer,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor)
        {
            SiteConfigs.Cache = memoryCache;
            SiteConfigs.GeneralLocalizer = generalLocalizer;
            SiteConfigs.Environment = environment;
            SiteConfigs.HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Initializes shared configuration settings in SiteConfigs
        /// </summary>
        private static void InitializeSharedSettings(
            JWTSettings jwtSettings,
            AppSecrets appSecrets,
            ApplicationSettings applicationSettings,
            EmailSettings emailSettings,
            MediaSettings mediaSettings,
            AwsSettings awsSettings,

            CategorySettings categorySettings,
            BlogSettings blogSettings,
            UserSettings userSettings)
        {
            SiteConfigs.JWTSettings = jwtSettings;
            SiteConfigs.ApplicationSettings = applicationSettings;
            SiteConfigs.EmailSettings = emailSettings;
            SiteConfigs.AppSecrets = appSecrets;
            SiteConfigs.MediaSettings = mediaSettings;
            SiteConfigs.AwsSettings = awsSettings;
            SiteConfigs.CategorySettings = categorySettings;
            SiteConfigs.BlogSettings = blogSettings;
            SiteConfigs.UserSettings = userSettings;
        }
    }

    /// <summary>
    /// Extension methods for registering the ConfigMiddleware
    /// </summary>
    public static class ConfigMiddlewareExtensions
    {
        /// <summary>
        /// Adds the ConfigMiddleware to the application pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseConfigMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ConfigMiddleware>();
        }
    }
}