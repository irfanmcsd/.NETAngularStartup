using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace NETAngularApp.Areas.api.Controllers
{
    /// <summary>
    /// API controller for user management operations including CRUD, authentication, and user actions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the UserController
        /// </summary>
        public UserController(
            ApplicationDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Loads users with optional pagination and filtering
        /// </summary>
        /// <returns>
        /// Returns either:
        /// - List of users with total count (for paginated requests)
        /// - Quick data without pagination (when skip_record_stats is true)
        /// - Report data placeholder (when RenderReport is selected)
        /// </returns>
        [HttpPost("load")]
        public async Task<ActionResult> Load()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var entity = JsonConvert.DeserializeObject<UserQueryEntity>(json);

                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // Handle report generation request
                if (UtilityHelper.IsSelected(entity.RenderReport))
                {
                    return Ok(new { status = "success", message = "Report integration later" });
                }

                List<ApplicationUser> posts = new();
                int records = 0;

                // Load quick data without pagination
                if (entity.skip_record_stats)
                {
                    var result = await UsersBLL.LoadItemsAsync(_context, entity);
                    posts = result ?? new List<ApplicationUser>();
                    return Ok(new { status = "success", posts });
                }

                // Load paginated data
                records = await UsersBLL.CountItemsAsync(_context, entity);
               
                posts = records > 0
                    ? await UsersBLL.LoadItemsAsync(_context, entity) ?? new List<ApplicationUser>()
                    : new List<ApplicationUser>();

                return Ok(new { status = "success", posts, records });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("Load", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Gets detailed information for a single user
        /// </summary>
        /// <returns>
        /// Returns user details if found, otherwise returns error message
        /// </returns>
        [HttpPost("getinfo")]
        public async Task<ActionResult> GetInfo()
        {
            try
            {
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var entity = JsonConvert.DeserializeObject<UserQueryEntity>(json);

                if (entity == null || (string.IsNullOrEmpty(entity.Id) && string.IsNullOrEmpty(entity.Slug)))
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                var users = await UsersBLL.LoadItemsAsync(_context, entity);

                if (users == null || users.Count == 0)
                {
                    return NotFound(new { status = "error", message = "No record found!" });
                }

                return Ok(new { status = "success", record = users[0] });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("GetInfo", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Processes user creation or updates
        /// </summary>
        /// <returns>
        /// Returns success status with created/updated user data or error message
        /// </returns>
        [HttpPost("proc")]
        public async Task<ActionResult> ProcessUser(CreateUserAccount entity)
        {
            try
            {
                // var json = await new StreamReader(Request.Body).ReadToEndAsync();
                // var entity = JsonConvert.DeserializeObject<CreateUserAccount>(json);

                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // Create new account
                if (string.IsNullOrEmpty(entity.Id))
                {
                    var roleName = GetRoleName(entity.account_type);
                    var user = new ApplicationUser
                    {
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        UserName = entity.Email,
                        Email = entity.Email,
                        Type = entity.Type,
                        CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                        IsEnabled = Types.ActionTypes.Enabled
                    };

                    await UsersBLL.CreateAccount(
                        _context,
                        _userManager,
                        _roleManager,
                        user,
                        entity.password,
                        roleName,
                        true);

                    return Ok(new
                    {
                        status = "success",
                        message = "Account created!",
                        record = entity
                    });
                }

                // Update existing account
                await UsersBLL.UpdateProfileAsync(_context, entity);
                return Ok(new { status = "success", record = entity });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessUser", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }


        [HttpPost("update_avatar")]
        public async Task<ActionResult> UpdateAvatar(CreateUserAccount entity)
        {
            try
            {                
                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // Update existing account
                await UsersBLL.UpdateAvatarAsync(_context, entity);

                return Ok(new { status = "success", record = entity });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessUser", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }


        [HttpPost("change_pass")]
        public async Task<ActionResult> ChangePassword(CreateUserAccount entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // Find user by ID
                var user = await _userManager.FindByIdAsync(entity.Id);
                if (user == null)
                {
                    return Ok(new { status = "success", message = "User not found" });
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Reset password
                var result = await _userManager.ResetPasswordAsync(user, token, entity.password);

                if (!result.Succeeded)
                {
                    return Ok(new { status = "success", message = "Password change failed" });
                    
                }
                return Ok(new { status = "success", message = "Password changed successfully", record = entity });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessUser", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        [HttpPost("change_email")]
        public async Task<ActionResult> ChangeEmail(CreateUserAccount entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // Update existing account
                await UsersBLL.UpdateAvatarAsync(_context, entity);

                return Ok(new { status = "success", record = entity });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessUser", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        [HttpPost("change_role")]
        public async Task<ActionResult> ChangeRole(CreateUserAccount entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                // write code to update user role (identity)
                var user = await _userManager.FindByIdAsync(entity.Id);
                if (user == null)
                {
                    return Ok(new { status = "success", message = "User not found" });
                }

                // Validate requested role exists
                var roleExists = await _roleManager.RoleExistsAsync(entity.role);
                if (!roleExists)
                {
                    return Ok(new { status = "success", message = $"Role '{entity.role}' does not exist" });
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove existing roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return Ok(new { status = "success", message = "Failed to remove existing roles" });
                }

                // Add new role
                var addResult = await _userManager.AddToRoleAsync(user, entity.role);
                if (!addResult.Succeeded)
                {
                    return Ok(new { status = "success", message = "Failed to add new role" });
                }

                return Ok(new { status = "success", message = $"User role changed to {entity.role} successfully", record = entity });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessUser", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        /// <summary>
        /// Performs batch actions on multiple users
        /// </summary>
        [HttpPost("action")]
        public async Task<ActionResult> ProcessActions(List<ApplicationUser> actionList)
        {
            try
            {
                //var json = await new StreamReader(Request.Body).ReadToEndAsync();
                // var actionList = JsonConvert.DeserializeObject<List<ApplicationUser>>(json);

                if (actionList == null)
                {
                    return BadRequest(new { status = "error", message = "Invalid input data" });
                }

                await UsersBLL.ProcessApiActionsAsync(_context, actionList);
                return Ok(new { status = "success", message = "Action performed successfully" });
            }
            catch (Exception ex)
            {
                var errorObj = await LogErrorAsync("ProcessActions", ex);
                return StatusCode(500, new
                {
                    status = "error",
                    message = $"Error occurred while processing your request. ErrorID: {errorObj.Id}"
                });
            }
        }

        #region Helper Methods

        /// <summary>
        /// Maps account type to role name
        /// </summary>
        private static string GetRoleName(byte accountType)
        {
            return accountType switch
            {
                1 => "Agent",
                2 => "Admin",
                _ => "User"
            };
        }

        /// <summary>
        /// Logs errors to the database
        /// </summary>
        private async Task<ErrorLogs> LogErrorAsync(string methodName, Exception ex)
        {
            return await ErrorLogsBLL.Add(_context, new ErrorLogs
            {
                Description = $"Error in {methodName}",
                Url = $"{nameof(UserController)} -> {methodName}()",
                StackTrace = ex.StackTrace
            });
        }

        #endregion
    }

    /// <summary>
    /// Extended user model for account creation with additional properties
    /// </summary>
    public class CreateUserAccount : ApplicationUser
    {
        /// <summary>
        /// Determines the account type and associated role
        /// 0 = User (default), 1 = Agent, 2 = Admin
        /// </summary>
        public byte account_type { get; set; }

        /// <summary>
        /// Password for new account creation
        /// </summary>
        public string password { get; set; } = string.Empty;

        /// <summary>
        /// Password for new account creation
        /// </summary>
        public string role { get; set; } = string.Empty;

    }
}