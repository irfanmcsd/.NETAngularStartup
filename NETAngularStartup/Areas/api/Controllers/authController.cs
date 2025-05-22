using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NETAngularApp.Areas.api.Controllers
{
    /// <summary>
    /// API controller for handling user authentication and JWT token generation.
    /// Provides core authorization services for the application.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    // [AutoValidateAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the AuthController.
        /// </summary>
        public AuthController(
            ApplicationDBContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token for authorization.
        /// </summary>
        /// <returns>
        /// Returns JWT token on successful authentication along with user information.
        /// Returns error message if authentication fails.
        /// </returns>
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var credentials = JsonConvert.DeserializeObject<UserQueryEntity>(json);

                if (credentials == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid credentials format" });
                }

                // Validate credentials and get user info
                var userInfo = await ValidateAndGetUserInfo(credentials);
                if (userInfo == null)
                {
                    return Unauthorized(new { status = "error", message = "Invalid credentials" });
                }

                // Check admin role if required
                if (credentials.IsAdmin && !await IsAdminUser(userInfo))
                {
                    return Unauthorized(new { status = "error", message = "Admin access required" });
                }

                // Generate JWT token
                var tokenResult = await GenerateJwtToken(userInfo);
                if (!tokenResult.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { status = "error", message = tokenResult.Message });
                }

                // Set cookie authentication for browser access
                await SetAuthCookie(userInfo);

                return Ok(new
                {
                    status = "success",
                    token = tokenResult.Token,
                    expiration = tokenResult.Expiration,
                    post = userInfo,
                    role = await _userManager.GetRolesAsync(userInfo)
                });
            }
            catch (Exception ex)
            {
                await LogAuthenticationError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = "error", message = "An error occurred during authentication" });
            }
        }

        #region Private Helper Methods

        private async Task<ApplicationUser?> ValidateAndGetUserInfo(UserQueryEntity credentials)
        {
            // Email/password authentication
            if (!string.IsNullOrEmpty(credentials.Email) && !string.IsNullOrEmpty(credentials.Password))
            {
                var result = await _signInManager.PasswordSignInAsync(
                    credentials.Email,
                    credentials.Password.Trim(),
                    isPersistent: false,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    return null;
                }

                return await _userManager.FindByEmailAsync(credentials.Email);
            }

            // Development mode authentication
            if (ApplicationSettings.Environment == "Development" && !string.IsNullOrEmpty(credentials.UserId))
            {
                var users = await UsersBLL.LoadItemsAsync(_context, new UserQueryEntity
                {
                    Id = credentials.UserId,
                    AdvanceFilter = false
                });

                return users?.FirstOrDefault();
            }

            // Standard authentication
            return await _userManager.GetUserAsync(User);
        }

        private async Task<bool> IsAdminUser(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles != null &&
                  (roles.Contains("Admin") || roles.Contains("SuperAdmin"));
        }

        private async Task<(bool Success, string? Token, DateTime? Expiration, string? Message)>
            GenerateJwtToken(ApplicationUser user)
        {

            var jwtKey = JWTSettings.SecretKey;
            // var jwtKey = UtilityHelper.GetKeyValue(JWTSettings.SecretKey, null);
            if (string.IsNullOrEmpty(jwtKey))
            {
                return (false, null, null, "JWT configuration error");
            }

            var authClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,
                    DateTime.UtcNow.AddDays(30).ToString("MMM ddd yyyy HH:mm:ss tt"))
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JWTSettings.Issuer,
                audience: JWTSettings.Audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
            );

            return (true, new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo, null);
        }

        private async Task SetAuthCookie(ApplicationUser user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });
        }

        private async Task LogAuthenticationError(Exception ex)
        {
            await ErrorLogsBLL.Add(_context, new ErrorLogs
            {
                Description = "Authentication Error",
                Url = $"{nameof(AuthController)}.{nameof(Authenticate)}",
                StackTrace = ex.ToString()
            });
        }

        #endregion
    }
}