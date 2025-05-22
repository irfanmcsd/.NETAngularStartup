// Primary namespace imports
using AngleSharp;
using AngleSharp.Io; // AngleSharp I/O components for document loading
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Middlewares;
using DevCodeArchitect.Services;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using reCAPTCHA.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

/*********************************
 * Configuration Setup
 *********************************/
// Configuration loading hierarchy
builder.Configuration
    .AddJsonFile("appsettings.json")                // Base configuration
    .AddEnvironmentVariables();                     // Environment overrides

// Validate critical configuration values immediately
ValidateEssentialSecrets(builder.Configuration);

/*********************************
 * AngleSharp.Io Configuration
 *********************************/

// So the AngleSharp configuration block should look like this:
builder.Services.AddSingleton<IBrowsingContext>(provider =>
    BrowsingContext.New(Configuration.Default
        .WithDefaultLoader()                        // Enable document loading
        .WithLocaleBasedEncoding()                  // Auto-detect encoding from locale
    ));

/*********************************
 * Service Registration
 *********************************/

// Strongly-typed configuration bindings
RegisterConfigurationSections(builder);

// Database Configuration (SQLite by default, production override available)
ConfigureDatabase(builder);

// Core Application Services
builder.Services
    .AddScoped<IUserService, UserService>()
    .AddSingleton<EmailTemplateService>()
    .AddHttpContextAccessor();

// Email Service
builder.Services.AddTransient<ICustomEmailSender, CustomEmailSender>();


// Authentication & Authorization Setup
ConfigureAuth(builder);

// Setup CORS Policy
var allowedHosts = builder.Configuration.GetSection("AllowedHosts").Get<string[]>();
if (allowedHosts != null &&  allowedHosts.Length > 0)
{
    //string[] origins = new string[] { "http://localhost:4200" };
    builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(allowedHosts)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    }));
}


// Setup Memory Cache
builder.Services.AddMemoryCache();

// Rate Limiting Policies
builder.Services.AddRateLimiter(limiter =>
{
    limiter.AddFixedWindowLimiter("ForgotPassword", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromHours(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});


builder.Services.AddScoped<ResourceService>();
// Setup Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure request localization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = CultureUtil.SupportedCultures();
    var defaultLanguage = builder.Configuration["ApplicationSettings:Localization:DefaultCulture"] ?? "en";
    options.SetDefaultCulture(defaultLanguage)
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    // Clear other providers if you want query string to be the only source
    options.RequestCultureProviders.Clear();

    // Add QueryString provider first
    //options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    options.RequestCultureProviders.Insert(0, new UrlCultureProvider());

});

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization();

/*********************************
 * Application Building
 *********************************/
var app = builder.Build();

// Middleware Pipeline
ConfigureMiddlewarePipeline(app);

// Route Configuration
ConfigureRoutes(app);

app.Run();


/*********************************
 * Supporting Methods
 *********************************/
void ValidateEssentialSecrets(Microsoft.Extensions.Configuration.IConfiguration config)
{
    // Validate JWT configuration
    var jwtSecret = config["JwtSettings:SecretKey"];
    if (string.IsNullOrEmpty(jwtSecret))
    {
        throw new InvalidOperationException("JWT_SECRET environment variable is not set");
    }

    // Validate reCAPTCHA configuration
    var recaptchaSiteKey = config["RechapchaSettings:SiteKey"];
    if (string.IsNullOrEmpty(recaptchaSiteKey) || recaptchaSiteKey == "{RECAPTCHA_SITE_KEY}")
    {
        throw new InvalidOperationException("RECAPTCHA_SITE_KEY configuration is invalid");
    }

    // Register reCAPTCHA service
    var recaptchaSettings = builder.Configuration.GetSection("RechapchaSettings");
    builder.Services.AddRecaptcha(recaptchaSettings);
}

void RegisterConfigurationSections(WebApplicationBuilder builder)
{
    // Strongly-typed configuration bindings
    builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
    builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<AppSecrets>(builder.Configuration.GetSection("Secrets"));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<MediaSettings>(builder.Configuration.GetSection("MediaSettings"));
    builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AwsSettings"));
    builder.Services.Configure<CategorySettings>(builder.Configuration.GetSection("CategorySettings"));
    builder.Services.Configure<BlogSettings>(builder.Configuration.GetSection("BlogSettings"));
    builder.Services.Configure<UserSettings>(builder.Configuration.GetSection("UserSettings"));

    builder.Services.Configure<SecurityHeadersConfig>(builder.Configuration.GetSection("SecurityHeaders"));
    
    // ... add other sections as needed ...
}

void ConfigureDatabase(WebApplicationBuilder builder)
{
    // SQLite default configuration
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Production database override (when configured)
    /*var mainConnection = builder.Configuration.GetValue<string>("ConnectionStrings:MainConnection");
    if (!string.IsNullOrEmpty(mainConnection))
    {
        var Host = builder.Configuration["Secrets:Database:Host"];
        var Name = builder.Configuration["Secrets:Database:Name"];
        var User = builder.Configuration["Secrets:Database:User"];
        var Password = builder.Configuration["Secrets:Database:Password"];
        ValidateDatabaseSecrets(Host, Name, User, Password);

        var formattedConn = FormatConnectionString(mainConnection, Host, Name, User, Password);
        builder.Services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(formattedConn)); // Can be changed to PostgreSQL/MySQL
    }*/
}

// for production
/*
void ValidateDatabaseSecrets(string? Host, string? Name, string? User, string? Password)
{    
    if (string.IsNullOrEmpty(Host)) throw new ArgumentNullException(nameof(Host));
    if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
    if (string.IsNullOrEmpty(User)) throw new ArgumentNullException(nameof(User));
    if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password));
}

string FormatConnectionString(string template, string? Host, string? Name, string? User, string? Password) =>
    template.Replace("{SERVER}", Host)
            .Replace("{DB}", Name)
            .Replace("{USER}", User)
            .Replace("{PWD}", Password);
*/

void ConfigureAuth(WebApplicationBuilder builder)
{
    // Identity Core Configuration
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDBContext>()
        .AddDefaultTokenProviders();

   
    // Google Provider
    var clientID = builder.Configuration["Secrets:OAuth:Google:ClientId"];
    var clientSecret = builder.Configuration["Secrets:OAuth:Google:ClientSecret"];
    if (!string.IsNullOrEmpty(clientID) && !string.IsNullOrEmpty(clientSecret))
    {
        builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = clientID;
            googleOptions.ClientSecret = clientSecret;
        });
    }

    // Facebook Provider
    /*var FB_ID = builder.Configuration["Secrets:OAuth:Facebook:FB_ID"];
    var FB_Secret = builder.Configuration["Secrets:OAuth:Facebook:FB_SECRET"];
    if (!string.IsNullOrEmpty(FB_ID) && !string.IsNullOrEmpty(FB_Secret))
    {
        builder.Services.AddAuthentication().AddFacebook(fbOptions =>
        {
            fbOptions.AppId = FB_ID;
            fbOptions.AppSecret = FB_Secret;

        });
    }

    // Twitter Provider
    var ConsumerKey = builder.Configuration["Secrets:OAuth:Twitter:ConsumerKey"];
    var ConsumerSecret = builder.Configuration["Secrets:OAuth:Twitter:ConsumerSecret"];
    if (!string.IsNullOrEmpty(FB_ID) && !string.IsNullOrEmpty(FB_Secret))
    {
        builder.Services.AddAuthentication().AddTwitter(twOption =>
        {
            twOption.ConsumerKey = ConsumerKey;
            twOption.ConsumerSecret = ConsumerSecret;

        });
    }*/
   
    // Add JWT + Cookie Authentication
    var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
    if (!string.IsNullOrEmpty(jwtSecret))
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "JWT_OR_COOKIE";
            options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        })
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/en/login";
            options.LogoutPath = "/en/signout";
            options.AccessDeniedPath = "/en/access-denied";
            options.ExpireTimeSpan = TimeSpan.FromDays(150);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = JWTSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = JWTSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                if (context.Request != null && context.Request.Headers != null)
                {
                    string? authorization = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return "Bearer";
                }

                // otherwise always check for cookie auth
                return "Cookies";
            };
        });
    }
}

void ConfigureMiddlewarePipeline(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Add security headers middleware
    app.UseMiddleware<SecurityHeadersMiddleware>();

    app.UseRequestLocalization();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCookiePolicy();
    app.UseRateLimiter();
    app.UseConfigMiddleware();
}

void ConfigureRoutes(WebApplication app)
{
    #region Blog Routes

    // post page
    app.MapControllerRoute(
        name: "blogs_tags_page",
        pattern: "{culture}/post/{title}",
        defaults: new { controller = "blogs", action = "post" });

    // blog categories
    app.MapControllerRoute(
        name: "blogs_tags_page",
        pattern: "{culture}/blog/tags/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "tags" });

    // blog categories
    app.MapControllerRoute(
        name: "blogs_category_page",
        pattern: "{culture}/blog/categories/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "categories" });

    // search blogs
    app.MapControllerRoute(
        name: "search_blogs_page",
        pattern: "{culture}/blog/q/{term}/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    app.MapControllerRoute(
        name: "search_blogs",
        pattern: "{culture}/blog/q/{term}",
        defaults: new { controller = "blogs", action = "index" });

    // blog by label or tags
    app.MapControllerRoute(
        name: "label_blogs_page",
        pattern: "{culture}/blog/tag/{label}/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    app.MapControllerRoute(
        name: "label_blogs",
        pattern: "{culture}/blog/tag/{label}",
        defaults: new { controller = "blogs", action = "index" });

    // blog by author
    app.MapControllerRoute(
        name: "author_blogs_page",
        pattern: "{culture}/blog/author/{user_slug}/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    app.MapControllerRoute(
        name: "author_blogs",
        pattern: "{culture}/blog/author/{label}",
        defaults: new { controller = "blogs", action = "index" });

    // featured blogs
    app.MapControllerRoute(
        name: "feature_blogs_page",
        pattern: "{culture}/blog/type/{featured}/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    app.MapControllerRoute(
        name: "featured_blogs",
        pattern: "{culture}/blog/type/{featured}",
        defaults: new { controller = "blogs", action = "index" });

    // /blog/home -> pagination
    app.MapControllerRoute(
        name: "blog_first_layer_category_page",
        pattern: "{culture}/blog/{categoryname}/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    // /blog/home
    app.MapControllerRoute(
        name: "blog_first_layer_category",
        pattern: "{culture}/blog/{categoryname}",
        defaults: new { controller = "blogs", action = "index" });


    // /blog/p-45
    app.MapControllerRoute(
        name: "default_blog_page",
        pattern: "{culture}/blog/p-{pagenumber}",
        defaults: new { controller = "blogs", action = "index" });

    app.MapControllerRoute(
        name: "default_blog_page",
        pattern: "{culture}/blog",
        defaults: new { controller = "blogs", action = "index" });

    #endregion

    #region Sub Accounts

    app.MapControllerRoute(
        name: "sub_acc",
        pattern: "{culture}/sub_acc/{user_id}/{*url}",
        defaults: new { controller = "subacc", action = "index" });

    app.MapControllerRoute(
        name: "sub_acc",
        pattern: "{culture}/sub_acc/{user_id}",
        defaults: new { controller = "subacc", action = "index" });
    #endregion

    #region User Account

    app.MapControllerRoute(
        name: "user",
        pattern: "{culture}/account/{*url}",
        defaults: new { controller = "account", action = "index" });

    app.MapControllerRoute(
        name: "user",
        pattern: "{culture}/account/",
        defaults: new { controller = "account", action = "index" });

    #endregion


    app.MapControllerRoute(
        name: "doc",
        pattern: "{culture}/doc/{term}",
        defaults: new { controller = "Doc", action = "index" });

    app.MapControllerRoute(
        name: "pages",
        pattern: "{culture}/{page}",
        defaults: new { controller = "Pages", action = "index" });

    app.MapControllerRoute(
        name: "language_handler",
        pattern: "{culture}",
        defaults: new { controller = "Home", action = "index" });


    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}
