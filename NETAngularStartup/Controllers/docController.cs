using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using reCAPTCHA.AspNetCore;

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


            return View();
        }
    }
}
