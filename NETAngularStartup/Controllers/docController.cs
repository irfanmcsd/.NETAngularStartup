using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.SDK;
using DevCodeArchitect.Services;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using reCAPTCHA.AspNetCore;
using System.Threading.Tasks;

namespace MagicTradeBot.Controllers
{
    public class docController : Controller
    {
        private readonly ApplicationDBContext _context;
        public docController(
         ApplicationDBContext context
        )
        {         
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index(string? culture, string term)
        {
            ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;
            if (string.IsNullOrEmpty(term))
                return Redirect("/" + ViewData["culture"] + "/docs");

            ViewData["term"] = term;
            ViewData["metaUrl"] = ApplicationSettings.Domain.Backend + ViewData["culture"] + "/doc/" + term;

            return View();
        }

        public async Task<IActionResult> SiteMap(string? culture)
        {
            var _culture = string.IsNullOrEmpty(culture) ? "en" : culture;
            var sitemap_feeds = await CategoryFeeds.generateGoogleSitemap(_context, new DevCodeArchitect.Entity.CategoryQueryEntity() { LoadAll = true, IsCache = true, type = DevCodeArchitect.Entity.CategoryEnum.Types.Documentation, IsEnabled = DevCodeArchitect.Entity.Types.ActionTypes.Enabled, Culture = _culture }, "doc");
            if (!string.IsNullOrEmpty(sitemap_feeds))
                return this.Content(sitemap_feeds);
            else
                return this.Content("Invalid Response");
        }

        
    }
}
