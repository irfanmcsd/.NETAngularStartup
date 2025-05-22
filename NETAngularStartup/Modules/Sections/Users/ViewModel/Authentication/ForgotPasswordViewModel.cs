using DevCodeArchitect.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevCodeArchitect.Entity
{
    /// <summary>
    /// Represents the data required to initiate a password reset process.
    /// </summary>
    /// <remarks>
    /// This view model is used in the "Forgot Password" workflow to validate and process
    /// the user's email address for password recovery. It ensures:
    /// 1. The email field is not empty
    /// 2. The provided value is a properly formatted email address
    /// </remarks>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the email address associated with the account needing password reset.
        /// </summary>
        /// <value>
        /// A valid email address in standard format (user@domain.com).
        /// </value>
        /// <example>user@example.com</example>
        [Required(ErrorMessage = "The email address is required to recover your account.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address format (e.g., user@example.com).")]
        [Display(Name = "Email Address", Prompt = "Enter your registered email")]
        [EmailExists(ErrorMessage = "This email is not registered in our system.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
    }
}

public class EmailExistsAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        // This method is required to override the base class's IsValid method.
        // However, since we are using asynchronous validation, we leave this empty.
        return true;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Synchronous fallback for compatibility
        var email = value as string;
        if (string.IsNullOrEmpty(email))
            return ValidationResult.Success;

        var userService = validationContext.GetService<IUserService>();
        if (userService != null && !userService.EmailExists(email).GetAwaiter().GetResult())
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
