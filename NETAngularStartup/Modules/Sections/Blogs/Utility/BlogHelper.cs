

using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;
/// <summary>
/// Core class for handling almost every type of request and prepare property listings
/// </summary>
public class BlogHelper
{

    public static async Task<BlogListViewModel> ProcessRequest(ApplicationDBContext context, BlogListingQueryModel entity)
    {
        // broad search
        string _broad_search_query = "";
        if (!string.IsNullOrEmpty(entity.Term))
        {
            _broad_search_query = HtmlSanitizer.StripHtml(entity.Term);
        }

        var ListEntity = new BlogListViewModel()
        {
            IsListStats = true,
            IsListNav = true,
            QueryOptions = new BlogQueryEntity()
            {
                Term = _broad_search_query,
                Culture = entity.Culture
            },
            NoRecordFoundText = "No Blogs Found!"
        };

        var _category_sub_term = "";
        if (!string.IsNullOrEmpty(entity.CategoryChild4))
            _category_sub_term = entity.CategoryChild4;
        if (!string.IsNullOrEmpty(entity.CategoryChild3))
            _category_sub_term = _category_sub_term + "/" + entity.CategoryChild3;
        if (!string.IsNullOrEmpty(entity.CategoryChild2))
            _category_sub_term = _category_sub_term + "/" + entity.CategoryChild2;
        if (!string.IsNullOrEmpty(entity.CategoryChild1))
            _category_sub_term = _category_sub_term + "/" + entity.CategoryChild1;

        if (!string.IsNullOrEmpty(entity.CategoryName))
        {
            if (!string.IsNullOrEmpty(_category_sub_term))
                _category_sub_term = _category_sub_term + "/" + entity.CategoryName;
            else
                _category_sub_term = entity.CategoryName;
        }

        if (!string.IsNullOrEmpty(_category_sub_term))
            ListEntity.QueryOptions.CategoryName = _category_sub_term;

        // set pagesize
        if (ApplicationSettings.Pagination.DefaultPageSize > 0)
            ListEntity.QueryOptions.PageSize = ApplicationSettings.Pagination.DefaultPageSize;

        // user slug
        if (!string.IsNullOrEmpty(entity.UserSlug))
            ListEntity.QueryOptions.UserSlug = entity.UserSlug;

        // label
        if (!string.IsNullOrEmpty(entity.Label))
            ListEntity.QueryOptions.Tags = entity.Label;

        // page number
        if (entity.PageNumber != null && entity.PageNumber > 0)
            ListEntity.QueryOptions.PageNumber = entity.PageNumber.Value;

        // orderby + filter 
        var _order = "blog.id desc";
        // var _selectedOrder = "Recent"; // Removed unused variable
        if (entity.Order != null)
        {
            switch (entity.Order)
            {
                // Add more cases here as needed
                // Example:
                // case "highest":
                //     _order = "blog.price desc";
                //     break;
                // case "lowest":
                //     _order = "blog.price asc";
                //     break;
            }
        }
        ListEntity.QueryOptions.Order = _order;

        // featured clause
        if (entity.Featured != null && entity.Featured != Types.FeaturedTypes.All)
        {
            ListEntity.QueryOptions.IsFeatured = entity.Featured.Value;
        }

        // filter cases e.g
        if (entity.Filter != null)
        {
            switch (entity.Filter)
            {
                case "featured":
                    ListEntity.QueryOptions.IsFeatured = Types.FeaturedTypes.Featured;
                    break;
                    // Add more filter cases if required
            }
        }

        // bread items 
        BlogBreadItemHelper.Generate(ListEntity, entity);

        // if search not enabled
        if (entity.SearchSource == 0)
        {
            // Set page heading
            BlogTitleHelper.Generate(ListEntity, entity);
            // Set page meta title
            BlogTitleHelper.GenerateMetaTitle(ListEntity, entity);
            // Set page meta description
            BlogTitleHelper.GenerateMetaDescription(ListEntity, entity);


            // set pagination links
            ListEntity.DefaultUrl = PaginationUtil.SetDefaultUrl();
            ListEntity.PaginationUrl = PaginationUtil.PaginationUrl(entity.Culture ?? "en");

            // set dropdown filters, order by option settings

            // set cache
            setCache(ListEntity, entity);
        }
        else
        {
            // search param mappings
        }

        // set grid, list based on cookie value selection
        /*if (!string.IsNullOrEmpty(SiteConfigurations.ListCookieName))
        {
            var list_selection_cookie = UtilityHelper.ReadCookie(SiteConfigurations.ListCookieName);
            if (list_selection_cookie != null)
            {
                if (list_selection_cookie == "1")
                    ListEntity.ListType = ListType.List;
                else
                    ListEntity.ListType = ListType.Grid;
            }
        }*/

        // response settings
        // Removed: if (entity.Response == null) entity.Response = Types.ListResponse.View;
        // Types.ListResponse is not nullable, so this check is unnecessary

        if (entity.Response == Types.ListResponse.View
            || entity.Response == Types.ListResponse.JSON
            //|| entity.Response == Types.ListResponse.Half_Map
            //|| entity.Response == Types.ListResponse.Full_Map
            )
        {
            ListEntity.TotalRecords = await BlogsBLL.CountItems(context, ListEntity.QueryOptions);
            if (ListEntity.TotalRecords != null && ListEntity.TotalRecords > 0)
            {
                ListEntity.DataList = await BlogsBLL.LoadItems(context, ListEntity.QueryOptions);
            }
        }
        else if (entity.Response == Types.ListResponse.Google || entity.Response == Types.ListResponse.Bing)
        {
            // google / bing sitemap
            ListEntity.QueryOptions.PageSize = 50000;
            switch (entity.Response)
            {
                case Types.ListResponse.Google:
                    // Google Feed
                    ListEntity.DataStr = await BlogFeeds.generateGoogleSitemap(context, ListEntity.QueryOptions);
                    break;
                case Types.ListResponse.Bing:
                    // Bing Feed
                    // RSS 2.0 supported by Google Bing
                    ListEntity.DataStr = await BlogFeeds.generateRSS(context, ListEntity.QueryOptions);
                    break;
            }
        }
        else
        {
            switch (entity.Response)
            {
                case Types.ListResponse.ATOM:
                    // ATOM Feed
                    ListEntity.DataStr = await BlogFeeds.generateATOM(context, ListEntity.QueryOptions);
                    break;
                case Types.ListResponse.RSS:
                    // RSS Feed
                    ListEntity.DataStr = await BlogFeeds.generateRSS(context, ListEntity.QueryOptions);
                    break;
            }
        }

        return ListEntity;
    }


    private static void setCache(BlogListViewModel listentity, BlogListingQueryModel entity)
    {
        if (listentity != null && listentity.QueryOptions != null)
        {
            if (entity.PageNumber == 1 && entity.SearchSource == 0
                && entity.UserSlug == null
                && entity.Order == null && entity.Filter == null)
            {
                listentity.QueryOptions.IsCache = true;
            }
        }
    }

}


/* Copyright © 2025, Mediasoftpro All rights reserved.
 * For inquiries and more information, please contact us at:
 * Email: clouddevarchitect@outlook.com
 * Website: www.devcodearchitect.com
 */