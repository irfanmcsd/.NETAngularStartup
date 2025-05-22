using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DevCodeArchitect.Entity;

/// <summary>
/// ViewModel for company account creation, extending the base account creation with company-specific properties
/// </summary>
public class CreateCompanyAccountViewModel : CreateAccountViewModel
{
    /// <summary>
    /// Gets or sets the company name (required)
    /// </summary>
    [Required(ErrorMessage = "Company name is required")]
    [Display(Name = "Company Name")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Company name must be between 2 and 100 characters")]
    [ValidCompanyName(ErrorMessage = "Company name can only contain letters, numbers, spaces, and .-_ characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Validates that a company name contains only allowed characters (letters, numbers, spaces, .-_)
/// and doesn't contain restricted words like "admin"
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidCompanyNameAttribute : ValidationAttribute
{
    private static readonly Regex ValidCharactersRegex =
        new Regex(@"^[a-zA-Z0-9\s\.\-_]+$", RegexOptions.Compiled);

    private static readonly string[] RestrictedWords =
        { "admin", "administrator", "root", "system" };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string companyName)
        {
            return new ValidationResult("Company name must be a string");
        }

        // Check for restricted words
        if (RestrictedWords.Any(word =>
            companyName.Contains(word, StringComparison.OrdinalIgnoreCase)))
        {
            return new ValidationResult("Company name cannot contain restricted words");
        }

        // Check for valid characters
        if (!ValidCharactersRegex.IsMatch(companyName))
        {
            return new ValidationResult(ErrorMessage ??
                "Company name can only contain letters, numbers, spaces, and .-_ characters");
        }

        return ValidationResult.Success;
    }
}