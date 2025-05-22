using System.ComponentModel.DataAnnotations;

namespace MediaSoft.Models
{
    /// <summary>
    /// ViewModel containing configuration for external login providers
    /// </summary>
    public class ExternalLoginProviderViewModel
    {
        /// <summary>
        /// The URL to return to after successful authentication
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;

        /// <summary>
        /// Flag indicating whether this is for a sign-up flow (true) or sign-in flow (false)
        /// </summary>
        public bool IsSignup { get; set; } = true; // Changed to PascalCase for C# convention
    }

    /// <summary>
    /// ViewModel for handling external login registration and authentication
    /// </summary>
    public class ExternalLoginViewModel
    {
        /// <summary>
        /// The unique username for the account
        /// </summary>
        /// <remarks>
        /// Must be 6-15 characters, lowercase alphanumeric with underscores/hyphens allowed
        /// </remarks>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Username must be between {2} and {1} characters")]
        [RegularExpression(
            @"^[a-z0-9_-]{5,15}$",
            ErrorMessage = "Username can only contain lowercase letters, numbers, underscores and hyphens")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty; // Initialize with default

        /// <summary>
        /// The user's email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The account password
        /// </summary>
        /// <remarks>
        /// Must be at least 6 characters with at least 1 digit and 1 special character
        /// </remarks>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be between {2} and {1} characters", MinimumLength = 6)]
        [RegularExpression(
            @"^(?=.*\d)(?=.*[\W]).{6,}$", // Improved regex
            ErrorMessage = "Password must contain at least 1 digit and 1 special character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation field (must match Password)
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// The user's first name (optional)
        /// </summary>
        [StringLength(50, ErrorMessage = "First name cannot exceed {1} characters")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// The user's last name (optional)
        /// </summary>
        [StringLength(50, ErrorMessage = "Last name cannot exceed {1} characters")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
    }
}