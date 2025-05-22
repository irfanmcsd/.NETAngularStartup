using DevCodeArchitect.Entity;

namespace DevCodeArchitect.Utilities;

/// <summary>
/// Provides URL generation utilities for user-related routes in the application
/// </summary>
/// <remarks>
/// Centralizes URL construction logic for user directory, company listings,
/// location-based searches, and featured user filters.
/// </remarks>
public static class UserUrls
{
    #region Base URLs

    /// <summary>
    /// Gets the base directory URL for user listings
    /// </summary>
    /// <returns>The relative URL path for the user directory</returns>
    public static string GetDirectoryUrl() => "agent";

    /// <summary>
    /// Gets the base URL for the agent finder section
    /// </summary>
    /// <returns>The relative URL path for the agent finder</returns>
    public static string GetUrl() => "/agent-finder";

    #endregion

    #region Company URLs

    /// <summary>
    /// Generates a URL for company-specific user listings
    /// </summary>
    /// <param name="companySlug">The company's URL slug identifier</param>
    /// <returns>
    /// Formatted URL path: "/agent-finder/company/{companySlug}"
    /// </returns>
    public static string GetCompanyUrl(string? companySlug)
        => $"/agent-finder/company/{companySlug}";

    #endregion

    #region Location URLs

    /// <summary>
    /// Generates a URL for place-specific user listings
    /// </summary>
    /// <param name="placeTerm">The place name or search term</param>
    /// <returns>
    /// Formatted URL path: "/agent-finder/place/{placeTerm}"
    /// </returns>
    public static string GetPlaceUrl(string? placeTerm)
        => $"/agent-finder/place/{placeTerm}";

    /// <summary>
    /// Generates a URL for country-specific user listings
    /// </summary>
    /// <param name="country">The country name</param>
    /// <returns>
    /// Formatted URL path: "/agent-finder/country/{country}"
    /// </returns>
    public static string GetCountryUrl(string? country)
        => $"/agent-finder/country/{country}";

    /// <summary>
    /// Generates a URL for state/province-specific user listings within a country
    /// </summary>
    /// <param name="state">The state/province name</param>
    /// <param name="country">The containing country name</param>
    /// <returns>
    /// Formatted URL path: "/agent-finder/state/{country}/{state}"
    /// </returns>
    public static string GetCountryStateUrl(string? state, string? country)
        => $"/agent-finder/state/{country}/{state}";

    #endregion

    #region Featured URLs

    /// <summary>
    /// Generates a URL for featured user listings
    /// </summary>
    /// <param name="featured">The featured type filter</param>
    /// <returns>
    /// Formatted URL path: "/agent-finder/type/{featuredType}"
    /// </returns>
    public static string GetFeaturedUrl(Types.FeaturedTypes? featured)
        => $"/agent-finder/type/{Types.ParseFeaturedTypes(featured)}";

    #endregion

    #region Breadcrumb Generation

    /// <summary>
    /// Processes and adds location-based breadcrumb items to the collection
    /// </summary>
    /// <param name="breadItems">The breadcrumb collection to populate</param>
    /// <param name="query">The query parameters containing location filters</param>
    /// <remarks>
    /// Handles three scenarios:
    /// 1. Place term filtering
    /// 2. Country and state filtering
    /// 3. Country-only filtering
    /// </remarks>
    public static void ProcessPlacesBreadItems(List<BreadItem> breadItems, UserListingQueryModel query)
    {
        if (breadItems == null) throw new ArgumentNullException(nameof(breadItems));
        if (query == null) throw new ArgumentNullException(nameof(query));

        /* Implementation examples (commented out):
        
        // Place term filtering
        if (!string.IsNullOrEmpty(query.PlaceTerm))
        {
            breadItems.Add(new BreadItem 
            { 
                Url = LocationUrls.GetUrl(), 
                Title = "Places" 
            });
            breadItems.Add(new BreadItem 
            { 
                Url = GetPlaceUrl(query.PlaceTerm), 
                Title = UtilityHelper.SetTitle(query.PlaceTerm) 
            });
        }
        // Country + state filtering
        else if (!string.IsNullOrEmpty(query.State) && !string.IsNullOrEmpty(query.Country))
        {
            breadItems.Add(new BreadItem { Url = LocationUrls.GetUrl(), Title = "Places" });
            breadItems.Add(new BreadItem 
            { 
                Url = GetCountryUrl(query.Country), 
                Title = UtilityHelper.SetTitle(query.Country) 
            });
            breadItems.Add(new BreadItem 
            { 
                Url = GetCountryStateUrl(query.State, query.Country), 
                Title = UtilityHelper.SetTitle(query.State) 
            });
        }
        // Country-only filtering
        else if (!string.IsNullOrEmpty(query.Country))
        {
            breadItems.Add(new BreadItem { Url = LocationUrls.GetUrl(), Title = "Places" });
            breadItems.Add(new BreadItem 
            { 
                Url = GetCountryUrl(query.Country), 
                Title = UtilityHelper.SetTitle(query.Country) 
            });
        }
        */
    }

    #endregion
}