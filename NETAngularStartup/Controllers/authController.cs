/*using AngleSharp.Css;
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Identity;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using reCAPTCHA.AspNetCore;

namespace RealEstateApp.Controllers
{
    public class authController : Controller
    {
        ApplicationDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRecaptchaService _recaptchaService;
        public authController(
            ApplicationDBContext context, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IRecaptchaService recaptchaService
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager; 
            _recaptchaService = recaptchaService;
        } 

        public async Task<IActionResult> signout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult Login()
        {
            ViewData["Title"] = "Sign In";

            return View();
        }



        // POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                // rechapcha validation
                var rechapcha = await _recaptchaService.Validate(Request);
                if (!rechapcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid chapcha, please try again.");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.IsEnabled == (byte)Types.ActionTypes.Disabled)
                    {
                        ModelState.AddModelError(string.Empty, "Account is suspended, please contact site administrator for more help.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(model.Email.Trim(), model.Password.Trim(), model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles != null)
                        {
                            if (UtilityHelper.isInRole(roles, "Admin") || UtilityHelper.isInRole(roles, "SuperAdmin"))
                            {
                                return Redirect("/admin");
                            }
                            else
                            {
                                return Redirect("/account");
                            }
                        }
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Account Locked!");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Credentials");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Credentials");
                    return View(model);
                }
             
            }

            return View(model);
        }


        // GET
        // Instead of /auth/createaccount, make it /auth/register
        public IActionResult Register()
        {
            ViewData["Title"] = "Create Account";

            return View();
        }

        // POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // rechapcha validation
                var rechapcha = await _recaptchaService.Validate(Request);
                if (!rechapcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid chapcha, please try again.");
                    return View(model);
                }

                var slug = UtilityHelper.GenerateSlug(model.Name);
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.Name,
                    CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                    IsEnabled = Types.ActionTypes.Enabled,
                    Slug = slug
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Redirect("/auth/activate");
                }
            }

            return View(model);
        }

        // GET
        public IActionResult CreateAgent()
        {
            ViewData["Title"] = "Create Agent or Service Provider Account";

            return View();
        }

        // POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAgent(CreateAgentViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.Type == 0)
                {
                    ModelState.AddModelError(string.Empty, "Please select service provider.");
                    return View(model);
                }

                // rechapcha validation
                var rechapcha = await _recaptchaService.Validate(Request);
                if (!rechapcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid chapcha, please try again.");
                    return View(model);
                }

                var slug = UtilityHelper.GenerateSlug(model.Name);
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.Name,
                    CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                    IsEnabled = Types.ActionTypes.Enabled,
                    Type = model.Type,
                    Slug = slug
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // add role
                    var roleName = "Agent";
                    var roleExist = await _roleManager.RoleExistsAsync("Agent");
                    if (!roleExist)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole()
                        {
                            Name = "Agent",
                            Description = "Agent",
                            CreatedDate = UtilityHelper.TimeZoneOffsetDateTime(),
                            IPAddress = ""
                        });
                    }

                    // assign role
                    await _userManager.AddToRoleAsync(user, roleName);

                    return Redirect("/auth/activate");
                }
            }

            return View(model);
        }


        public IActionResult CreateCompany()
        {
            ViewData["Title"] = "Create Company Account";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(CreateCompanyAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // rechapcha validation
                var rechapcha = await _recaptchaService.Validate(Request);
                if (!rechapcha.success)
                {
                    ModelState.AddModelError(string.Empty, "Invalid chapcha, please try again.");
                    return View(model);
                }

                var slug = UtilityHelper.GenerateSlug(model.Name);
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.Name,
                    CreatedAt = UtilityHelper.TimeZoneOffsetDateTime(),
                    IsEnabled = Types.ActionTypes.Enabled,
                    Slug = slug
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    // add role
                    var roleName = "Company";
                    var roleExist = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole()
                        {
                            Name = roleName,
                            Description = roleName,
                            CreatedDate = UtilityHelper.TimeZoneOffsetDateTime(),
                            IPAddress = ""
                        });
                    }

                    // assign role
                    await _userManager.AddToRoleAsync(user, roleName);

                    // create company & associate created user as primary user
                    /*await CompaniesBLL.AddViaSignup(_context, new Companies()
                    {
                        title = model.CompanyName,
                        description = "",
                        phone = model.Phone,
                        email = model.Email,
                        isenabled = (byte)Types.ActionTypes.Enabled,
                        isapproved = (byte)Types.ActionTypes.Enabled,
                    }, user.Id);*/
/*

                    return Redirect("/auth/activate");
                }
            }

            return View(model);
        }


        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Forgot Password";

            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult Activate()
        {
            return View();
        }
    }
}
*/