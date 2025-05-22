using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;

/// <summary>
/// Core service for processing user-related requests and preparing user listings
/// </summary>
/// <remarks>
/// Handles filtering, sorting, pagination, and view preparation for user listings.
/// Integrates with caching, breadcrumb generation, and UI presentation logic.
/// </remarks>
public class UserHelper
{
    /// <summary>
    /// Processes user listing requests and prepares the view model
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="entity">Incoming query parameters</param>
    /// <returns>Populated UserListViewModel with results and metadata</returns>
    public static async Task<UserListViewModel> ProcessRequestAsync(
        ApplicationDBContext context,
        UserListingQueryModel entity)
    {
        // Initialize the view model with default values
        var listEntity = InitializeViewModel(entity);

        // Apply search term filtering
        ApplySearchTerm(entity, listEntity);

        // Configure pagination settings
        ConfigurePagination(entity, listEntity);

        // Apply location filters
        ApplyLocationFilters(entity, listEntity);

        // Configure sorting options
        ConfigureSorting(entity, listEntity);

        // Apply featured filters
        ApplyFeaturedFilters(entity, listEntity);

        // Apply additional custom filters
        ApplyCustomFilters(entity, listEntity);

        // Configure UI elements
        ConfigureUIElements(listEntity, entity);

        // Load data if not a search request
        if (entity.Source == 0)
        {
            ConfigurePageMetadata(listEntity, entity);
        }

        // Set display type based on cookie preference
        SetDisplayType(listEntity);

        // Execute data retrieval
        await LoadUserDataAsync(context, listEntity);

        return listEntity;
    }

    #region Initialization Methods

    private static UserListViewModel InitializeViewModel(UserListingQueryModel entity)
    {
        return new UserListViewModel
        {
            IsListStats = true,
            IsListNav = true,
            QueryOptions = new UserQueryEntity(),
            NoRecordFoundText = "No User Found!"
        };
    }

    private static void ApplySearchTerm(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        if (!string.IsNullOrEmpty(entity.Term) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.Term = HtmlSanitizer.StripHtml(entity.Term);
        }
    }

    #endregion

    #region Configuration Methods

    private static void ConfigurePagination(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        if (ApplicationSettings.Pagination.DefaultPageSize > 0 && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.PageSize = ApplicationSettings.Pagination.DefaultPageSize;
        }

        if (entity.PageNumber != null && entity.PageNumber > 0 && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.PageNumber = entity.PageNumber.Value;
        }
    }

    private static void ApplyLocationFilters(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        if (entity.LocationId != null && entity.LocationId > 0 && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.LocationId = entity.LocationId.Value;
        }

        if (!string.IsNullOrEmpty(entity.CompanySlug) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.CompanySlug = entity.CompanySlug;
        }

        if (!string.IsNullOrEmpty(entity.City) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.City = entity.City;
        }

        if (!string.IsNullOrEmpty(entity.State) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.State = entity.State;
        }

        if (!string.IsNullOrEmpty(entity.Country) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.Country = entity.Country;
        }

        if (!string.IsNullOrEmpty(entity.Zip) && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.Zip = entity.Zip;
        }
    }

    private static void ConfigureSorting(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        var order = "user.created_at desc";
        var selectedOrder = "Recent";

        if (entity.Order != null)
        {
            switch (entity.Order.ToLower())
            {
                // Add additional sorting cases as needed
                /*
                case "highest":
                    selectedOrder = "Highest Price";
                    order = "property.price desc";
                    break;
                case "lowest":
                    selectedOrder = "Lowest Price";
                    order = "property.price asc";
                    break;
                */
            }
        }

        if (listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.Order = order;
        }
        
        listEntity.SelectedOrder = selectedOrder;
    }

    private static void ApplyFeaturedFilters(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        if (entity.Featured != null && entity.Featured != Types.FeaturedTypes.All && listEntity.QueryOptions != null)
        {
            listEntity.QueryOptions.IsFeatured = entity.Featured.Value;
        }
    }

    private static void ApplyCustomFilters(UserListingQueryModel entity, UserListViewModel listEntity)
    {
        if (entity.Filter != null && listEntity.QueryOptions != null)
        {
            switch (entity.Filter.ToLower())
            {
                case "featured":
                    listEntity.QueryOptions.IsFeatured = Types.FeaturedTypes.Featured;
                    break;
                    // Add additional filter cases as needed
            }
        }
    }

    #endregion

    #region UI Configuration Methods

    private static void ConfigureUIElements(UserListViewModel listEntity, UserListingQueryModel entity)
    {
        UserBreadItemHelper.Generate(listEntity, entity);
    }

    private static void ConfigurePageMetadata(UserListViewModel listEntity, UserListingQueryModel entity)
    {
        listEntity.DefaultUrl = PaginationUtil.SetDefaultUrl();
        listEntity.PaginationUrl = PaginationUtil.PaginationUrl("en");
        SetCache(listEntity, entity);
    }

    private static void SetDisplayType(UserListViewModel listEntity)
    {
        /*if (!string.IsNullOrEmpty(SiteConfigurations.ListCookieName))
        {
            var listSelectionCookie = UtilityHelper.ReadCookie(SiteConfigurations.ListCookieName);
            listEntity.ListType = listSelectionCookie == "1" ? ListType.List : ListType.Grid;
        }*/
    }

    #endregion

    #region Data Loading Methods

    private static async Task LoadUserDataAsync(ApplicationDBContext context, UserListViewModel listEntity)
    {
        if (listEntity.QueryOptions != null)
        {
            listEntity.TotalRecords = await UsersBLL.CountItemsAsync(context, listEntity.QueryOptions);

            if (listEntity.TotalRecords > 0)
            {
                listEntity.DataList = await UsersBLL.LoadItemsAsync(context, listEntity.QueryOptions);
            }
        }
    }

    #endregion

    #region Cache Management

    private static void SetCache(UserListViewModel listEntity, UserListingQueryModel entity)
    {
        if (listEntity?.QueryOptions == null) return;

        bool shouldCache = entity.PageNumber == 1 &&
                         entity.Source == 0 &&
                         string.IsNullOrEmpty(entity.CompanySlug) &&
                         string.IsNullOrEmpty(entity.Category.ToString()) &&
                         string.IsNullOrEmpty(entity.Order) &&
                         string.IsNullOrEmpty(entity.Filter);

        listEntity.QueryOptions.IsCache = shouldCache;
    }

    #endregion
}