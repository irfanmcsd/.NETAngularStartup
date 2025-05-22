using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity
{
    /// <summary>
    /// Represents the data required for resetting a user's password.
    /// This model is used when a user needs to create a new password after requesting a password reset.
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// Required field and must be in a valid email format.
        /// </summary>
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// Must meet complexity requirements:
        /// - Minimum 7 characters
        /// - Maximum 20 characters
        /// - At least one number
        /// - At least one special character
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password must be between 7 and 20 characters.", MinimumLength = 7)]
        [RegularExpression(
            pattern: @"(?=.{7,})(?=(.*\d){1,})(?=(.*\W){1,})",
            ErrorMessage = "Password must contain at least one number and one special character.")]
        [Display(Name = "New Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password confirmation.
        /// Should match the Password field (validation typically handled by Compare attribute in the controller).
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user identifier.
        /// This is typically a hidden field populated by the password reset process.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the password reset token.
        /// This is typically a hidden field populated by the password reset link.
        /// </summary>
        public string? Code { get; set; }
    }
}