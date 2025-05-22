using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace NETAngularApp.Controllers
{
    /// <summary>
    /// Controller for handling account-related operations
    /// </summary>
    [Authorize]
    public class AccountController : Controller  // Changed to PascalCase
    {
        private readonly ILogger<AccountController> _logger;

        // Use dependency injection for logger
        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Main account panel view with culture support
        /// </summary>
        /// <param name="culture">Optional culture code (default: "en")</param>
        [HttpGet]  // Explicit HTTP method
        public IActionResult Index(string culture = "en")  // Default parameter value
        {
            try
            {
                _logger.LogInformation("Accessing account panel for user {User}", User.Identity?.Name);

                // Set view data using constants to avoid magic strings
                ViewData[ViewDataKeys.Culture] = culture.Trim().ToLower();
                ViewData[ViewDataKeys.Title] = "Account Panel";
                ViewData[ViewDataKeys.BaseUrl] = Url.Content("~/" + culture + "/account/");  // Use Url helper

                // Add user information to ViewData
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Authenticated user has no ID claim");
                    return RedirectToAction("Error", "Home");
                }
                ViewData[ViewDataKeys.UserID] = userId;
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