using DevCodeArchitect.DBContext;
using DevCodeArchitect.Services;
using DevCodeArchitect.Utilities;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DevCodeArchitect.Extensions;

/// <summary>
/// Provides extension methods for user email-related operations.
/// </summary>
public static class UserEmailExtensions
{

    public static async Task SendPasswordResetEmailAsync(this ICustomEmailSender emailSender,
    ApplicationDBContext context,
    EmailTemplateService templateService,
    string email,
    string name,
    string resetLink,
    string culture = "en")
    {
        // Validate required parameters
        if (emailSender == null)
            throw new ArgumentNullException(nameof(emailSender));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(resetLink))
            throw new ArgumentException("Reset link cannot be empty", nameof(resetLink));

        // Only proceed if email functionality is enabled
        if (!EmailSettings.Enabled)
        {
            // Log that email functionality is disabled if needed
            return;
        }

        string subject = $"Password Reset Request - {ApplicationSettings.PageCaption}";

        // Get localized email template
        var replacements = new Dictionary<string, string>
        {
            ["PageCaption"] = ApplicationSettings.PageCaption ?? string.Empty,
            ["UserName"] = name,
            ["ConfirmationLink"] = resetLink,
            ["SupportLink"] = EmailSettings.SupportEmail ?? string.Empty,
            ["SupportEmail"] = EmailSettings.SupportEmail ?? string.Empty,
            //["ExpiryHours"] = "24" // or from config
        };

        string emailBody = templateService.RenderTemplate(
            EmailTemplateType.PasswordReset,
            replacements,
            CultureInfo.GetCultureInfo(culture));

        try
        {
            email = "sales@magictradebot.com";
            await emailSender.SendEmailAsync(context, email, subject, emailBody);
        }
        catch (Exception ex)
        {          

            throw; // Re-throw to handle in calling method
        }
    }

    public static async Task SendEmailConfirmationAsync(this ICustomEmailSender emailSender, ApplicationDBContext context, EmailTemplateService TemplateService, string Email, string Link, string Name, string Culture)
    {
        // Validate required parameters
        if (emailSender == null)
            throw new ArgumentNullException(nameof(emailSender));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("Email cannot be empty", nameof(Email));

        if (string.IsNullOrWhiteSpace(Link))
            throw new ArgumentException("Reset link cannot be empty", nameof(Link));

        // Only proceed if email functionality is enabled
        if (!EmailSettings.Enabled)
        {
            // Consider logging that email functionality is disabled
            return;
        }

        string Subject = $"Confirm Password - {ApplicationSettings.PageCaption}";

        await SendSmtpConfirmEmailAsync(emailSender, context, TemplateService, Email, Name, Subject, Link, Culture);


        // Use Mandrill if configured
        /*if (SiteCredentials.EnableMandrill && !string.IsNullOrEmpty(SiteCredentials.MandrillKey))
        {
            await SendMandrillConfirmResetEmailAsync(emailSender, context, Email, Name, Link, Subject);
        }
        else
        {
            await SendSmtpConfirmEmailAsync(emailSender, context, TemplateService, Email, Name, Subject, Link, Culture);
        }*/

    }

    /// <summary>
    /// Asynchronously sends a password reset email to the specified user.
    /// </summary>
    /// <param name="emailSender">The email sender service (extension method target)</param>
    /// <param name="context">The database context</param>
    /// <param name="email">The recipient's email address</param>
    /// <param name="name">The recipient's name</param>
    /// <param name="link">The password reset link</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <remarks>
    /// This method will:
    /// 1. Check if email functionality is enabled in site configurations
    /// 2. Use Mandrill service if configured, otherwise fall back to SMTP
    /// 3. Send an email with appropriate template/content
    /// </remarks>
    public static async Task ChangeEmailResetAsync(
        this ICustomEmailSender emailSender,
        ApplicationDBContext context,
        EmailTemplateService TemplateService,
        string email,
        string name,
        string link)
    {
        // Validate required parameters
        if (emailSender == null)
            throw new ArgumentNullException(nameof(emailSender));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(link))
            throw new ArgumentException("Reset link cannot be empty", nameof(link));

        // Only proceed if email functionality is enabled
        if (!EmailSettings.Enabled)
        {
            // Consider logging that email functionality is disabled
            return;
        }

        string subject = $"Reset Password - {ApplicationSettings.PageCaption}";
        await SendSmtpResetEmailAsync(emailSender, context, TemplateService, name, email, subject, link);

        // Use Mandrill if configured
        /*if (SiteCredentials.EnableMandrill && !string.IsNullOrEmpty(SiteCredentials.MandrillKey))
        {
            await SendMandrillResetEmailAsync(emailSender, context, email, name, link, subject);
        }
        else
        {
            await SendSmtpResetEmailAsync(emailSender, context, email, subject, link);
        }*/
    }


    /// <summary>
    /// Sends email using Mandrill service (Confirm Email)
    /// </summary>
    /*private static async Task SendMandrillConfirmResetEmailAsync(
        ICustomEmailSender emailSender,
        ApplicationDBContext context,
        string email,
        string name,
        string link,
        string subject)
    {
        const string mandrillTemplateName = "CONFIRM_EMAIL";

        var templateVariables = new Dictionary<string, dynamic>
        {
            { "NAME", name ?? string.Empty }, // Handle null name
            { "URL", link },
            { "EMAIL", email }
        };

        // Ensure SupportEmail and SmtpDisplayName have fallback values
        var fromEmail = EmailSettings.SupportEmail ?? "support@magictradebot.com";
        var displayName = EmailSettings.FromName ?? "Support Team";

        await emailSender.SendEmailAsync(
            context,
            email,
            fromEmail,
            displayName,
            subject,
            mandrillTemplateName,
            templateVariables);
    }*/


    /// <summary>
    /// Sends email using Mandrill service (Reset Email)
    /// </summary>
    /*private static async Task SendMandrillResetEmailAsync(
        ICustomEmailSender emailSender,
        ApplicationDBContext context,
        string email,
        string name,
        string link,
        string subject)
    {
        const string mandrillTemplateName = "EMAIL_RESET";

        var templateVariables = new Dictionary<string, dynamic>
        {
            { "NAME", name ?? string.Empty }, // Handle null name
            { "URL", link },
            { "EMAIL", email }
        };

        // Ensure SupportEmail and SmtpDisplayName have fallback values
        var fromEmail = EmailSettings.SupportEmail ?? "support@magictradebot.com";
        var displayName = EmailSettings.FromName ?? "Support Team";

        await emailSender.SendEmailAsync(
            context,
            email,
            fromEmail,
            displayName,
            subject,
            mandrillTemplateName,
            templateVariables);
    }*/

    /// <summary>
    /// Sends email using SMTP fallback (Confirm Email)
    /// </summary>
    private static async Task SendSmtpConfirmEmailAsync(
        ICustomEmailSender emailSender,
        ApplicationDBContext context,
        EmailTemplateService emailTemplateService,
        string email,
        string name,
        string subject,
        string link,
        string culture)
    {

        var replacements = new Dictionary<string, string>
        {
            ["PageCaption"] = ApplicationSettings.PageCaption ?? string.Empty,
            ["UserName"] = name,
            ["ConfirmationLink"] = link,
            ["SupportLink"] = EmailSettings.SupportEmail ?? string.Empty,
            ["SupportEmail"] = EmailSettings.SupportEmail ?? string.Empty,
            //["ExpiryHours"] = "24" // or from config
        };

        string emailBody = emailTemplateService.RenderTemplate(
            EmailTemplateType.EmailConfirmation,
            replacements,
            CultureInfo.GetCultureInfo(culture)); // Optional culture

        email = "sales@magictradebot.com";
        await emailSender.SendEmailAsync(context, email, subject, emailBody);
    }

    /// <summary>
    /// Sends email using SMTP fallback
    /// </summary>
    private static async Task SendSmtpResetEmailAsync(
        ICustomEmailSender emailSender,
        ApplicationDBContext context,
        EmailTemplateService emailTemplateService,
        string name,
        string email,
        string subject,
        string link,
        string culture = "en-US") 
    {

        var replacements = new Dictionary<string, string>
        {
            ["PageCaption"] = ApplicationSettings.PageCaption ?? string.Empty,
            ["UserName"] = name,
            ["ResetLink"] = link,
            ["SupportLink"] = EmailSettings.SupportEmail ?? string.Empty,
            ["SupportEmail"] = EmailSettings.SupportEmail ?? string.Empty,
        };

        string emailBody = emailTemplateService.RenderTemplate(
            EmailTemplateType.EmailConfirmation,
            replacements,
            CultureInfo.GetCultureInfo(culture));

        email = "sales@magictradebot.com";
        await emailSender.SendEmailAsync(context, email, subject, emailBody);
    }
}