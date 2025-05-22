using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.SDK;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace NETAngularApp.Controllers
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
            ViewData["pageTitle"] = ListEntity.HeadingTitle;

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
            ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;
            ViewData["term"] = term;
            return View();
        }
    }
}
