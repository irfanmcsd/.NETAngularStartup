using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity;

/// <summary>
/// ViewModel for user account creation, containing validation rules for user registration data
/// </summary>
/// <remarks>
/// This class is used to collect and validate user information during account registration.
/// It includes data annotations for client-side and server-side validation.
/// </remarks>
public class CreateAccountViewModel
{
    /// <summary>
    /// Gets or sets the user's full name
    /// </summary>
    /// <example>John Doe</example>
    [Display(Name = "Full Name")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the user's email address (required)
    /// </summary>
    /// <example>user@example.com</example>
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password (required)
    /// </summary>
    /// <remarks>
    /// Password must meet complexity requirements:
    /// - Minimum 7 characters
    /// - At least one number
    /// - At least one special character
    /// </remarks>
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 7,
        ErrorMessage = "Password must be 7-20 characters long")]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[\W]).{7,20}$",
        ErrorMessage = "Password must contain at least one number and one special character")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation (must match Password)
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}