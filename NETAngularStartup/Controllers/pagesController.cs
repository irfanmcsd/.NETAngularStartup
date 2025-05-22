using AngleSharp.Dom;
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Extensions;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Services;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using reCAPTCHA.AspNetCore;
using System.Security.Claims;



namespace NETAngularApp.Controllers
{
    /// <summary>
    /// Controller for handling various pages including authentication, static content, and external login flows
    /// </summary>
    public class PagesController : Controller // Changed to PascalCase to follow conventions
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomEmailSender _emailSender;
        private readonly IRecaptchaService _recaptcha;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<PagesController> _logger; // Added for logging
        private readonly EmailTemplateService _templateService;

        public PagesController(
            IOptions<RecaptchaSettings> recaptchaSettings,
            IRecaptchaService recaptcha,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ICustomEmailSender emailSender,
            ILogger<PagesController> logger,
            EmailTemplateService templateService)
        {
            _recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateService = templateService;
        }

        /// <summary>
        /// Main entry point for various static pages and authentication views
        /// </summary>
        /// <param name="culture">Language/culture code (e.g., "en", "fr")</param>
        /// <param name="page">The page identifier to load</param>
        /// <returns>Appropriate view based on the page parameter</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string? culture, string page)
        {
            try
            {
                if (string.IsNullOrEmpty(page))
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewData["page"] = page;
                ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;

                // Handle authentication pages differently
                if (page == "signin" || page == "signup" || page == "forgot")
                {
                    return HandleAuthPages(page);
                }
                else if (page == "signout")
                {
                    await _signInManager.SignOutAsync();
                }
                // Set page titles for static content pages
                ViewData["pageTitle"] = page.ToLower() switch
                {
                    "activate" => "Activate Account",
                    "signout" => "Sign Out",
                    "email_verified" => "Email Verification Completed",
                    "email_not_verified" => "Email Verification Failed",
                    "reset_pass" => "Reset Password",
                    "reset_pass_failed" => "Reset Password Failed",
                    "reset_pass_confirmed" => "Reset Passwrod Confirmed",
                    "faq" => "Frequently Asked Questions",
                    "features" => "Features",
                    "templates" => "Trading Templates",
                    "lab" => "Trading Strategy Builder Tool",
                    "demo" => "Demo",
                    "plan" => "Pricing",
                    "docs" => "Documentation",
                    "blogs" => "Latest News & Blogs",
                    "download" => "Download",
                    "feedback" => "Feedback",
                    "privacy" => "Privacy Policy",
                    "terms" => "Terms & Conditions",
                    "about" => "About Us",
                    "contact" => "Contact Us",
                    _ => "Page Not Found"
                };

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading page {Page}", page);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles authentication-related pages (signin, signup, forgot password)
        /// </summary>
        /// <param name="page">The auth page to handle</param>
        /// <returns>Appropriate view or redirect</returns>
        private IActionResult HandleAuthPages(string page)
        {
            ViewData["pageTitle"] = page switch
            {
                "signin" => "Sign In",
                "signup" => "Sign Up",
                "forgot" => "Forgot Password",
                _ => "Authentication"
            };

            // Redirect if already authenticated
            if (_signInManager.IsSignedIn(User))
            {
                return Redirect($"/{ViewData["culture"]}/account/");
            }

            return page switch
            {
                "signin" => View(new LoginViewModel()),
                "signup" => View(new CreateAccountViewModel()),
                "forgot" => View(new ForgotPasswordViewModel()),
                _ => View()
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string page, string culture = "en", string? returnUrl = null)
        {
            try
            {
                // Set view data for culture and page
                ViewData["page"] = string.IsNullOrEmpty(page) ? "signin" : page;
                ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;
                ViewData["ReturnUrl"] = returnUrl;

                // Validate model state
                if (!ModelState.IsValid)
                {
                    return RedirectToAuthPage(culture, page);
                }

                // Validate reCAPTCHA
                var recaptcha = await _recaptcha.Validate(model.RecaptchaResponse);

                if (!recaptcha.success || recaptcha.score < 0.5) // Adjust threshold (0.0-1.0)
                {
                    ModelState.AddModelError("", "reCAPTCHA validation failed");
                    return View(model);
                }
                /*var recaptcha = await _recaptcha.Validate(Request);
                if (!recaptcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Captcha");
                    return RedirectToAuthPage(culture, page);
                }*/

                // Find user by email or username
                var user = await FindUserForLoginAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Account Doesn't Exist");
                    return RedirectToAuthPage(culture, page);
                }

                // Validate user account status
                var accountStatusError = ValidateUserAccountStatusAsync(user);
                if (accountStatusError != null)
                {
                    ModelState.AddModelError(string.Empty, accountStatusError);
                    return RedirectToAuthPage(culture, page);
                }

                // Attempt to sign in
                var signInResult = await _signInManager.PasswordSignInAsync(
                    user.UserName ?? string.Empty,
                    model.Password,
                    isPersistent: true,
                    lockoutOnFailure: false);

                if (signInResult.Succeeded)
                {
                    // Log successful login
                    _logger.LogInformation("User {UserId} logged in successfully", user.Id);

                    // Track login IP
                    await TrackUserLoginIpAsync(user);

                    // Redirect to return URL or default account page
                    return RedirectToLocal(returnUrl ?? $"/{culture}/account/");
                }

                // Handle failed login
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return RedirectToAuthPage(culture, page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during login");
                return RedirectToAuthPage(culture, page);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateAccountViewModel model, string? page, string culture, string? returnUrl = null)
        {
            try
            {
                // Set view data for culture and page
                ViewData["page"] = string.IsNullOrEmpty(page) ? "signup" : page;
                ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;

                // Validate model state
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                // Validate reCAPTCHA
                var recaptcha = await _recaptcha.Validate(Request);
                if (!recaptcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Captcha");
                    return View("Index", model);
                }

                // Validate email format
                if (!IsValidEmail(model.Email))
                {
                    ModelState.AddModelError(nameof(model.Email), "Invalid email format");
                    return View("Index", model);
                }

                // Check if email already exists
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email is already registered");
                    return View("Index", model);
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                    FirstName = model.Name?.Trim(),
                    IsEnabled = Types.ActionTypes.Enabled,
                    Type = UserEnum.UserTypes.NormalUser
                };

                // Create user with password
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    AddIdentityErrorsToModelState(result.Errors);
                    return View("Index", model);
                }

                // Ensure "User" role exists and assign to new user
                await EnsureUserRoleExistsAndAssignAsync(user);

                // Generate email confirmation token and send email
                await SendEmailConfirmationAsync(user, culture);

                // Redirect 
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return Redirect($"/{ViewData["page"]}/activate");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during registration");
                return View("Index", model);
            }
        }

        #region Helper Methods

        /// <summary>
        /// Finds user by email or username for login purposes
        /// </summary>
        private async Task<ApplicationUser?> FindUserForLoginAsync(string emailOrUsername)
        {
            if (emailOrUsername.Contains("@"))
            {
                return await _userManager.FindByEmailAsync(emailOrUsername);
            }
            return await _userManager.FindByNameAsync(emailOrUsername);
        }

        /// <summary>
        /// Validates user account status (disabled, archived, email confirmed)
        /// </summary>
        private string? ValidateUserAccountStatusAsync(ApplicationUser user)
        {
            if (user.IsEnabled == Types.ActionTypes.Disabled)
            {
                _logger.LogWarning("Attempt to login to disabled account {UserId}", user.Id);
                return "Account Deactivated";
            }

            if (user.IsEnabled == Types.ActionTypes.Archive)
            {
                _logger.LogWarning("Attempt to login to archived account {UserId}", user.Id);
                return "Account Doesn't Exist";
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Attempt to login to unconfirmed email account {UserId}", user.Id);
                return "Activate your account by clicking on email sent to your account to continue.";
            }

            return null;
        }

        /// <summary>
        /// Redirects to the authentication page with culture and page parameters
        /// </summary>
        private IActionResult RedirectToAuthPage(string culture, string page)
        {
            return RedirectToAction(nameof(Index), new { culture, page });
        }

        /// <summary>
        /// Tracks the user's login IP address
        /// </summary>
        private async Task TrackUserLoginIpAsync(ApplicationUser user)
        {
            try
            {
                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    // Add your IP tracking logic here
                    // Example: await _userService.TrackLoginIpAsync(user.Id, ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking login IP for user {UserId}", user.Id);
            }
        }

        /// <summary>
        /// Validates email format using simple regex
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ensures "User" role exists and assigns it to the new user
        /// </summary>
        private async Task EnsureUserRoleExistsAndAssignAsync(ApplicationUser user)
        {
            const string roleName = "User";

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = "Standard application user",
                    CreatedDate = UtilityHelper.TimeZoneOffsetDateTime()
                });
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        /// <summary>
        /// Sends email confirmation to the new user
        /// </summary>
        private async Task SendEmailConfirmationAsync(ApplicationUser user, string? culture)
        {
            try
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Pages",
                    new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailConfirmationAsync(_context, _templateService, user.Email, callbackUrl, user.FirstName, culture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", user.Email);
            }
        }

        /// <summary>
        /// Adds identity errors to ModelState
        /// </summary>
        private void AddIdentityErrorsToModelState(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                // Map common identity errors to more user-friendly messages
                var message = error.Code switch
                {
                    "DuplicateUserName" => "Username is already taken",
                    "DuplicateEmail" => "Email is already registered",
                    "PasswordTooShort" => "Password must be at least 6 characters",
                    "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character",
                    "PasswordRequiresDigit" => "Password must contain at least one number",
                    "PasswordRequiresUpper" => "Password must contain at least one uppercase letter",
                    _ => error.Description
                };

                ModelState.AddModelError(string.Empty, message);
            }
        }

        #endregion

        /// <summary>
        /// Initiates external authentication flow (Google, Facebook, etc.)
        /// </summary>
        /// <param name="provider">Authentication provider name</param>
        /// <param name="culture">Language/culture code</param>
        /// <param name="page">Original page that initiated the request</param>
        /// <param name="returnUrl">URL to return to after authentication</param>
        /// <returns>Challenge result that redirects to external provider</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalSignup(string provider, string? culture, string? page, string? returnUrl = null)
        {
            try
            {
                var redirectUrl = Url.Action(nameof(ExternalSignupCallback), "Pages",
                    new { returnUrl, culture, page });

                var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                    provider, redirectUrl);

                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating external signup with {Provider}", provider);
                return RedirectToAction(nameof(Index), new { culture, page });
            }
        }

        /// <summary>
        /// Callback endpoint for external authentication providers
        /// </summary>
        /// <param name="culture">Language/culture code</param>
        /// <param name="page">Original page that initiated the request</param>
        /// <param name="returnUrl">URL to return to after authentication</param>
        /// <param name="remoteError">Error from external provider, if any</param>
        /// <returns>Redirect to appropriate page based on authentication result</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalSignupCallback(
            string? culture, string? page, string? returnUrl = null, string? remoteError = null)
        {
            try
            {
                ViewData["page"] = string.IsNullOrEmpty(page) ? "signin" : page;
                ViewData["culture"] = string.IsNullOrEmpty(culture) ? "en" : culture;

                if (remoteError != null)
                {
                    _logger.LogWarning("External provider error: {Error}", remoteError);
                    TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                    return RedirectToAction(nameof(Index), new { culture, page = "signin" });
                }

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogWarning("Failed to get external login info");
                    TempData["ErrorMessage"] = "Error loading external login information.";
                    return RedirectToAction(nameof(Index), new { culture, page = "signin" });
                }

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("External provider didn't return email claim");
                    TempData["ErrorMessage"] = "The external provider didn't provide an email address.";
                    return RedirectToAction(nameof(Index), new { culture, page = "signin" });
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: true);
                    return RedirectToLocal(returnUrl);
                }

                // Create new user from external provider info
                var newUser = await CreateUserFromExternalProvider(info, email);
                if (newUser == null)
                {
                    return RedirectToAction(nameof(Index), new { culture, page = "signup" });
                }

                await _signInManager.SignInAsync(newUser, isPersistent: true);
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in external signup callback");
                TempData["ErrorMessage"] = "An error occurred during authentication.";
                return RedirectToAction(nameof(Index), new { culture, page = "signin" });
            }
        }

        /// <summary>
        /// Creates a new user account from external provider information
        /// </summary>
        /// <param name="info">External login information</param>
        /// <param name="email">User's email address</param>
        /// <returns>Created user or null if creation failed</returns>
        private async Task<ApplicationUser?> CreateUserFromExternalProvider(ExternalLoginInfo info, string email)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                FirstName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "User",
                IsEnabled = Types.ActionTypes.Enabled,
                Type = UserEnum.UserTypes.NormalUser,
                EmailConfirmed = true // Trust external provider's email verification
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                _logger.LogWarning("User creation failed: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return null;
            }

            // Ensure "User" role exists
            const string roleName = "User";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    Description = "Standard application user",
                    CreatedDate = UtilityHelper.TimeZoneOffsetDateTime()
                });
            }

            // Add user to role
            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Failed to add user to role: {Errors}",
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Add external login to user
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                _logger.LogWarning("Failed to add external login: {Errors}",
                    string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
            }

            return user;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("fixed")]
        // [ServiceFilter(typeof(RateLimitAttribute))] 
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string? culture = "en", string? page = "forgot")
        {
            // Set culture and page view data
            ViewData["culture"] = culture;
            ViewData["page"] = page;

            // Validate reCAPTCHA first
            var recaptcha = await _recaptcha.Validate(Request);
            if (!recaptcha.success)
            {
                _logger.LogWarning("Failed reCAPTCHA validation for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid CAPTCHA verification. Please try again.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Always return the same message whether user exists or not (security best practice)
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogInformation("Password reset requested for non-existent or unconfirmed email: {Email}", model.Email);

                    // Generic success message to prevent email enumeration
                    return RedirectToAction("ForgotPasswordConfirmation", new { email = model.Email, culture, page });
                }

                // Generate reset token with expiration
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Create callback URL with token
                var callbackUrl = Url.Action(
                    action: "ResetPassword",
                    controller: "Account",
                    values: new
                    {
                        culture,
                        page = "reset",
                        email = model.Email,
                        code = token
                    },
                    protocol: Request.Scheme);

                // Send email (fixed: pass required context and templateService)
                await _emailSender.SendPasswordResetEmailAsync(
                    _context,
                    _templateService,
                    user.Email ?? string.Empty,
                    user.FirstName ?? string.Empty,
                    callbackUrl ?? string.Empty,
                    culture ?? "en"
                );

                _logger.LogInformation("Password reset email sent to {Email}", user.Email);

                return RedirectToAction("ForgotPasswordConfirmation", new { email = model.Email, culture, page });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing password reset request for email: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation(string email, string? culture = "en", string? page = "forgot")
        {
            ViewData["culture"] = culture;
            ViewData["page"] = page;
            ViewData["Email"] = email; // For displaying in the view

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                // Log warning about missing parameters
                _logger.LogWarning("ConfirmEmail called with missing parameters - UserId: {UserId}, Code: {Code}", userId, code);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // Log error for non-existent user
                    _logger.LogError("User not found with ID: {UserId}", userId);
                    return RedirectToAction(nameof(Index), new { culture = "en", page = "email_verification_error" });
                }

                // Attempt to confirm the user's email
                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    // Log successful email confirmation
                    _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);

                    // Optionally: Send welcome email or perform post-confirmation actions
                    return RedirectToAction(nameof(Index), new
                    {
                        culture = "en",
                        page = "email_verified",
                        email = user.Email // Pass email for display if needed
                    });
                }
                else
                {
                    // Log email confirmation failure
                    _logger.LogError("Email confirmation failed for user: {UserId}. Errors: {Errors}",
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));

                    return RedirectToAction(nameof(Index), new
                    {
                        culture = "en",
                        page = "email_not_verified",
                        errors = result.Errors // Pass errors for display if needed
                    });
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                _logger.LogError(ex, "Error confirming email for user: {UserId}", userId);
                return RedirectToAction(nameof(Index), new
                {
                    culture = "en",
                    page = "error",
                    message = "An unexpected error occurred while confirming your email."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string culture, string? userId = null, string? code = null)
        {
            // Validate the reset code
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("ResetPassword called with invalid code");
                return RedirectToAction(nameof(Index), new
                {
                    culture = culture ?? "en",
                    page = "reset_pass_failed",
                    message = "Invalid or missing reset code"
                });
            }

            // Prepare view data for the reset password form
            var viewData = new Dictionary<string, object>
            {
                ["Code"] = code,
                ["User"] = userId,
                ["Culture"] = culture ?? "en"
            };

            // If userId is provided, try to get the user's email
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    viewData["Email"] = user.Email;
                    _logger.LogInformation("Preparing password reset for user: {Email}", user.Email);
                }
                else
                {
                    _logger.LogWarning("User not found for password reset: {UserId}", userId);
                }
            }

            return View("ResetPassword", viewData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string? page, string? culture)
        {
            // Set default culture if not provided
            culture = culture ?? "en";
            page = page ?? "signin";

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ResetPassword model validation failed");
                return View(model);
            }

            try
            {
                // Validate userId and code are not null or empty
                if (string.IsNullOrWhiteSpace(model.UserId))
                {
                    _logger.LogError("ResetPassword attempt with null or empty UserId");
                    ModelState.AddModelError(string.Empty, "User information is not valid.");
                    return View(model);
                }
                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    _logger.LogError("ResetPassword attempt with null or empty Code");
                    ModelState.AddModelError(string.Empty, "Reset code is missing or invalid.");
                    return View(model);
                }

                // Validate user exists
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    _logger.LogError("ResetPassword attempt for non-existent user: {UserId}", model.UserId);
                    ModelState.AddModelError(string.Empty, "User information is not valid.");
                    return View(model);
                }

                // Attempt password reset
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for user: {Email}", user.Email);

                    // Generate confirmation link (example implementation) => PasswordResetConfirmed
                    var confirmationLink = Url.Action(
                        action: nameof(Index),
                        controller: "Pages",
                        values: new { userId = user.Id, email = user.Email, culture, page },
                        protocol: Request.Scheme) ?? string.Empty;

                    // Send confirmation email (ensure non-null values)
                    await _emailSender.ChangeEmailResetAsync(
                        _context,
                        _templateService,
                        user.Email ?? string.Empty,
                        user.FirstName ?? string.Empty,
                        confirmationLink);

                    return RedirectToAction(nameof(Index), new
                    {
                        culture,
                        page = "reset_pass_confirmed",
                        email = user.Email
                    });
                }

                // Handle password reset failures
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning("Password reset error for {Email}: {Error}", user.Email, error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", model.UserId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return View(model);
            }
        }

        // Helper action for password reset confirmation
        [HttpGet]
        public IActionResult PasswordResetConfirmed(string? page, string? culture, string userId, string email)
        { 
            // Set default culture if not provided
            culture = culture ?? "en";
            page = page ?? "signin";

            _logger.LogInformation("Password reset confirmed for user: {Email}", email);
            ViewData["Email"] = email;
            return View("Index");
        }



        /// <summary>
        /// Safely redirects to a local URL or default page
        /// </summary>
        /// <param name="returnUrl">URL to redirect to</param>
        /// <returns>Redirect result</returns>
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect($"/{ViewData["culture"]}/account/");
        }
    }
}