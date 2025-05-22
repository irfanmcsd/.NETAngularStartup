namespace DevCodeArchitect.Utilities;

public class ApplicationSettings
{
    public static string Author { get; set; } = string.Empty;
    public static string PageCaption { get; set; } = string.Empty;
    public static string Environment { get; set; } = "Development"; // [Development|Staging|Production]
    public static ApplicationDomain Domain { get; set; } = new ApplicationDomain();
    public static AppLocalization Localization { get; set; } = new AppLocalization();
    public static AppPagination Pagination { get; set; } = new AppPagination();
    public static AppCache Caching { get; set; } = new AppCache();
    public static AppSecurity Security { get; set; } = new AppSecurity();

    public static int MaximumUrlCharacters { get; set; }

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
            PageSize = Pagination.DefaultPageSize,
            Currency = Localization.Currency
        };
    }

}

public class ApplicationDomain
{
    public string Backend { get; set; } = string.Empty;

    public string Frontend { get; set; } = string.Empty;
}

public class AppLocalization
{
    public string DefaultCulture { get; set; } = "en";
    public string Timezone { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}

public class AppPagination
{
    public int DefaultPageSize { get; set; }
    public int MaxPageSize { get; set; }
    public int VisiblePageCount { get; set; }
}

public class AppCache
{
    public bool Enabled { get; set; }
    public int Duration { get; set; }
    public string MaxCacheSize { get; set; } = string.Empty;
}

public class AppSecurity
{
    public bool RequireHTTPS { get; set; }
    public string CookiePolicy { get; set; } = string.Empty;
    public string ContentSecurityPolicy { get; set;} = string.Empty;
}