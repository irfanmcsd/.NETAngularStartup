using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity
{
    /// <summary>
    /// Represents the data required for user authentication.
    /// This model is used for the login view and form submission.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// This field is required and will be displayed as "Email Address" in the UI.
        /// </summary>
        [Required(ErrorMessage = "Email address is required.")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// This field is required and will be rendered as a password input field in the UI.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the authentication cookie should persist.
        /// When true, the login session will be remembered across browser sessions.
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [BindProperty(Name = "g-recaptcha-response")]
        public string RecaptchaResponse { get; set; } = string.Empty;
    }
}