using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;

/// <summary>
/// Helper class for generating breadcrumb navigation items for user listings
/// </summary>
/// <remarks>
/// Constructs hierarchical breadcrumb trails based on the current user listing context.
/// Handles home links, search results, company filters, location hierarchies, and pagination.
/// </remarks>
public static class UserBreadItemHelper
{
    /// <summary>
    /// Generates breadcrumb items for user listings based on query parameters
    /// </summary>
    /// <param name="listEntity">The view model to populate with breadcrumbs</param>
    /// <param name="query">The query parameters defining the current listing context</param>
    public static void Generate(UserListViewModel listEntity, UserListingQueryModel query)
    {
        if (listEntity == null) throw new ArgumentNullException(nameof(listEntity));
        if (query == null) throw new ArgumentNullException(nameof(query));

        var breadItems = new List<BreadItem>
        {
            // Base navigation items
            CreateBreadItem("/", "Home"),
            CreateBreadItem(UserUrls.GetUrl(), "Agents")
        };

        // Add search term breadcrumb if present
        // if (!string.IsNullOrEmpty(query.Term))
        // {
        //     breadItems.Add(CreateBreadItem(
        //         UserUrls.GetSearchUrl(query.Term),
        //         UtilityHelper.SetTitle(query.Term)));
        // }

        // Add company hierarchy breadcrumbs if filtered by company
        // if (!string.IsNullOrEmpty(query.CompanySlug))
        // {
        //     breadItems.Add(CreateBreadItem(CompanyUrls.GetUrl(), "Companies"));
        //     breadItems.Add(CreateBreadItem(
        //         UserUrls.GetCompanyUrl(query.CompanySlug),
        //         UtilityHelper.SetTitle(query.CompanySlug)));
        // }

        // Add location-based breadcrumbs
        UserUrls.ProcessPlacesBreadItems(breadItems, query);

        // Add featured filter breadcrumb if specified
        if (query.Featured != null)
        {
            breadItems.Add(CreateBreadItem(
                UserUrls.GetFeaturedUrl(query.Featured),
                UtilityHelper.SetTitle(Types.ParseFeaturedTypes(query.Featured))));
        }

        // Add pagination indicator if beyond first page
        if (query.PageNumber > 1)
        {
            breadItems.Add(CreateBreadItem("#", $"Page - {query.PageNumber}"));
        }

        listEntity.BreadItems = breadItems;
    }

    /// <summary>
    /// Creates a new BreadItem with the specified URL and title
    /// </summary>
    /// <param name="url">The navigation URL</param>
    /// <param name="title">The display title</param>
    /// <returns>A configured BreadItem instance</returns>
    private static BreadItem CreateBreadItem(string Url, string Title)
    {
        return new BreadItem
        {
            Url = Url,
            Title = Title
        };
    }
}