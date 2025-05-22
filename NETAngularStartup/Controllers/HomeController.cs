
using AngleSharp.Css;
using AngleSharp.Dom;
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Services;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETAngularApp.Models;
using System.Diagnostics;
using System.Globalization;


namespace NETAngularApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApplicationDBContext _context;
        private readonly ResourceService _resourceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly EmailTemplateService _templateService;

        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDBContext context, 
            ResourceService resourceService, 
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            EmailTemplateService templateService)
        {
            _logger = logger;
            _context = context;
            _resourceService = resourceService;

            _userManager = userManager;
            _roleManager = roleManager;
            _templateService = templateService;
        }
        public IActionResult Index(string? culture)
        {
            var key = JWTSettings.SecretKey;

            ViewData["Home"] = true;
            ViewData["page"] = "";
            ViewData["Culture"] = "en";
            if (!string.IsNullOrEmpty(culture))
            {
                ViewData["Culture"] = culture;
            }

            ViewData["pageTitle"] = _resourceService.GetValue("Meta_Home_Title");

            ViewData[ViewDataKeys.IsAuthenticated] = User.Identity?.IsAuthenticated;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}