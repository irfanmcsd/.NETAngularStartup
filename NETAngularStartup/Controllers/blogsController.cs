using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.SDK;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace MagicTradeBot.Controllers
{
    public class blogsController : Controller
    {
        ApplicationDBContext _context;

        public blogsController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(BlogListingQueryModel entity)
        {
            ViewData["culture"] = string.IsNullOrEmpty(entity.Culture) ? "en" : entity.Culture;
           
            var ListEntity = await BlogHelper.ProcessRequest(_context, entity);
            ViewData["pageIndex"] = "Blog"; // Represent blog section
            ViewData["pageTitle"] = ListEntity.MetaTitle;
            ViewData["metaDescription"] = ListEntity.MetaDescription;
            ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + UtilityHelper.AdjustSlash(ListEntity.DefaultUrl);
            ViewData["blogPage"] = UtilityHelper.RemoveCulturePrefix(UtilityHelper.AdjustSlash(ListEntity.DefaultUrl));

            if (entity.Response == Types.ListResponse.View || entity.Response == Types.ListResponse.Half_Map || entity.Response == Types.ListResponse.Full_Map)
                return View(ListEntity);
            else
            {
                // Google, Bing, ATOM, RSS Response
                if (!string.IsNullOrEmpty(ListEntity.DataStr))
                    return this.Content(ListEntity.DataStr);
                else
                    return this.Content("Invalid Response");
            }
        }

        public IActionResult post(string? culture, string? term)
        {
            ViewData["pageIndex"] = "Blog"; // Represent blog section
            ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;
            ViewData["b_term"] = term;
            ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/post/" + term;
            return View();
        }

        public IActionResult tags(string? culture, int? pagenumber)
        {
            if (pagenumber == null)
                pagenumber = 1;

            ViewData["pageIndex"] = "Blog"; // Represent blog section
            ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;
            ViewData["pageTitle"] = "Browse All Blog Tags | Discover Topics & Categories | " + ApplicationSettings.PageCaption;
            ViewData["metaDescription"] = "Explore all blog tags and labels to quickly find topics that interest you. Navigate articles by category and discover the latest insights on crypto trading automation.";
            if (pagenumber > 1)
                ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/blog/tags/p-" + pagenumber;
            else
                ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/blog/tags/";

            return View();
        }

        /* Generally not required but if you have functionality to display categories separately in a customized list */
        public IActionResult categories(string? culture, int? pagenumber)
        {
            if (pagenumber == null)
                pagenumber = 1;

            ViewData["pageIndex"] = "Blog"; // Represent blog section
            ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;

            if (pagenumber > 1)
                ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/blog/categories/p-" + pagenumber;
            else
                ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/blog/categories/";

            return View();
        }

        public async Task<IActionResult> SiteMap(string? culture)
        {
            var _culture = string.IsNullOrEmpty(culture) ? "en" : culture;
            var sitemap_feeds = await CategoryFeeds.generateGoogleSitemap(_context, new DevCodeArchitect.Entity.CategoryQueryEntity() { LoadAll = true, IsCache = true, type = DevCodeArchitect.Entity.CategoryEnum.Types.Blogs, IsEnabled = DevCodeArchitect.Entity.Types.ActionTypes.Enabled, Culture = _culture }, "blog");
            if (!string.IsNullOrEmpty(sitemap_feeds))
                return this.Content(sitemap_feeds);
            else
                return this.Content("Invalid Response");
        }
    }
}
