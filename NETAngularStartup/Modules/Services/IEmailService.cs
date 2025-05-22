using DevCodeArchitect.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCodeArchitect.Services
{
    /// <summary>
    /// Defines an email sending service contract with support for both simple messages
    /// and templated emails with variable substitution.
    /// </summary>
    public interface ICustomEmailSender
    {
        /// <summary>
        /// Sends a simple email message to a single recipient.
        /// </summary>
        /// <param name="context">The database context for logging and tracking purposes.</param>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The body content of the email (HTML or plain text).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations should handle their own error logging and retry policies.
        /// </remarks>
        Task SendEmailAsync(
            ApplicationDBContext context,
            string email,
            string subject,
            string message);

        /// <summary>
        /// Sends a templated email with support for merge variables and custom sender information.
        /// </summary>
        /// <param name="context">The database context for logging and tracking purposes.</param>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="fromEmail">The sender's email address.</param>
        /// <param name="displayName">The display name for the sender.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="templateName">The name of the email template to use.</param>
        /// <param name="mergeVariables">Dictionary of variables to substitute in the template (nullable).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations should provide default sender information if fromEmail is null or empty.
        /// The template processing should handle null mergeVariables gracefully.
        /// </remarks>
        Task SendEmailAsync(
            ApplicationDBContext context,
            string toEmail,
            string fromEmail,
            string displayName,
            string subject,
            string templateName,
            Dictionary<string, dynamic>? mergeVariables);
    }
}