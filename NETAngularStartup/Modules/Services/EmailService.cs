using DevCodeArchitect.DBContext;
using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCodeArchitect.Services
{
    /// <summary>
    /// Service for sending emails with support for both simple messages and templated emails.
    /// Implements the IEmailSender interface to provide email sending capabilities.
    /// </summary>
    public class CustomEmailSender : ICustomEmailSender
    {
        /// <summary>
        /// Sends a simple email message to a single recipient.
        /// </summary>
        /// <param name="context">Database context for logging or tracking purposes</param>
        /// <param name="email">Recipient email address</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="message">Email body content (HTML or plain text)</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public async Task SendEmailAsync(ApplicationDBContext context,
                                      string email,
                                      string subject,
                                      string message)
        {
            await MailProcess.Send_Mail(context, email, subject, message);
        }

        /// <summary>
        /// Sends a templated email with support for merge variables and custom sender information.
        /// </summary>
        /// <param name="context">Database context for logging or tracking purposes</param>
        /// <param name="to_email">Recipient email address</param>
        /// <param name="from_email">Sender email address (uses default if empty)</param>
        /// <param name="display_name">Sender display name</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="templateName">Name of the email template to use</param>
        /// <param name="mergeVariables">Dictionary of variables to merge into the template</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        /// <remarks>
        /// If from_email is not provided, the default SMTP email from SiteConfigurations will be used.
        /// The display name will also fall back to the default if from_email is not provided.
        /// </remarks>
        public async Task SendEmailAsync(ApplicationDBContext context,
                                      string to_email,
                                      string from_email,
                                      string display_name,
                                      string subject,
                                      string templateName,
                                      Dictionary<string, dynamic>? mergeVariables)
        {
            // Set default sender information if not provided
            var fromEmailDisplayName = display_name;
            if (string.IsNullOrEmpty(from_email) && !string.IsNullOrEmpty(EmailSettings.FromAddress))
            {
                from_email = EmailSettings.SupportEmail;
                fromEmailDisplayName = EmailSettings.FromName;
            }

            // Prepare recipient list
            var toEmails = new List<string> { to_email };

            // Send templated email
            await Utilities.Mandrill.EmailProcess.SendMail(
                context: context,
                fromEmail: from_email,
                fromName: fromEmailDisplayName,
                toMails: toEmails,
                subject: subject,
                templateName: templateName,
                mergeFields: mergeVariables);
        }
    }
}