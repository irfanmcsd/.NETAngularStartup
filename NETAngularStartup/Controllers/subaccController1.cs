using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace NETAngularApp.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class subaccController1 : Controller
    {
        private readonly ILogger<AccountController> _logger;

        // Use dependency injection for logger
        public subaccController1(ILogger<AccountController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(string? user_id, string culture = "en")
        {
            try
            {
                if (string.IsNullOrEmpty(user_id))
                {                  
                    return RedirectToAction("Error", "Home");
                }

                _logger.LogInformation("Accessing user account panel for user {User}", user_id);

                // Set view data using constants to avoid magic strings
                ViewData[ViewDataKeys.Culture] = culture.Trim().ToLower();
                ViewData[ViewDataKeys.Title] = "User Account Panel";
                ViewData[ViewDataKeys.BaseUrl] = Url.Content("~/" + culture + "/sub_acc/");  // Use Url helper

               
                ViewData[ViewDataKeys.UserID] = user_id;
                ViewData[ViewDataKeys.IsAuthenticated] = User.Identity?.IsAuthenticated;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading account panel");
                return RedirectToAction("Error", "Home");
            }

        }
    }
}
