using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;


namespace DevCodeArchitect.Utilities.Mandrill
{
    /// <summary>
    /// Provides email sending functionality using the Mandrill email service.
    /// Supports both simple HTML emails and templated emails with merge variables.
    /// </summary>
    public class EmailProcess
    {
        /// <summary>
        /// Sends an email with HTML content to multiple recipients
        /// </summary>
        /// <param name="context">Database context for error logging</param>
        /// <param name="fromEmail">Sender email address</param>
        /// <param name="fromName">Sender display name</param>
        /// <param name="toMails">List of recipient email addresses</param>
        /// <param name="subject">Email subject</param>
        /// <param name="htmlBody">HTML content of the email</param>
        /// <param name="async">Whether to send asynchronously (currently unused)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public static async Task SendMail(
            ApplicationDBContext context,
            string fromEmail,
            string fromName,
            List<string> toMails,
            string subject,
            string htmlBody,
            bool async = false)
        {
            try
            {
                if (string.IsNullOrEmpty(EmailSettings.Mandril.ApiKey))
                {
                    await LogMissingCredentialsError(context);
                    return;
                }

                var api = new MandrillApi(EmailSettings.Mandril.ApiKey);

                var email = new EmailMessage
                {
                    FromEmail = fromEmail,
                    FromName = fromName,
                    Subject = subject,
                    Html = htmlBody,
                    To = toMails.Select(mailTo => new EmailAddress(mailTo)).ToList()
                };

                var sendRequest = new SendMessageRequest(email);
                await api.SendMessage(sendRequest);
            }
            catch (Exception ex)
            {
                // Consider logging the actual error here
                await ErrorLogsBLL.Add(context, new ErrorLogs
                {
                    Description = $"Email sending failed: {ex.Message}",
                    Url = "EmailProcess.SendMail",
                    StackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Sends a templated email with merge variables to multiple recipients
        /// </summary>
        /// <param name="context">Database context for error logging</param>
        /// <param name="fromEmail">Sender email address</param>
        /// <param name="fromName">Sender display name</param>
        /// <param name="toMails">List of recipient email addresses</param>
        /// <param name="subject">Email subject</param>
        /// <param name="templateName">Name of the Mandrill template to use</param>
        /// <param name="mergeVariables">Dictionary of variables to merge into the template</param>
        /// <returns>List of email results or null if failed</returns>
        public static async Task<List<EmailResult>?> SendMail(
            ApplicationDBContext context,
            string? fromEmail,
            string? fromName,
            List<string> toMails,
            string subject,
            string templateName,
            Dictionary<string, dynamic>? mergeFields)
        {
            try
            {
                if (string.IsNullOrEmpty(EmailSettings.Mandril.ApiKey))
                {
                    await LogMissingCredentialsError(context);
                    return null;
                }

                var api = new MandrillApi(EmailSettings.Mandril.ApiKey);

                var email = new EmailMessage
                {
                    FromEmail = fromEmail,
                    FromName = fromName,
                    Subject = subject,
                    Merge = true,
                    MergeLanguage = "mailchimp",
                    To = toMails.Select(mailTo => new EmailAddress(mailTo)).ToList()
                };

                if (mergeFields != null)
                {
                    foreach (var variable in mergeFields)
                    {
                        email.AddGlobalVariable(variable.Key, variable.Value);
                    }
                }

                var sendRequest = new SendMessageTemplateRequest(email, templateName);
                return await api.SendMessageTemplate(sendRequest);
            }
            catch (Exception ex)
            {
                await ErrorLogsBLL.Add(context, new ErrorLogs
                {
                    Description = $"Templated email sending failed: {ex.Message}",
                    Url = "EmailProcess.SendMail (Templated)",
                    StackTrace = ex.StackTrace
                });
                throw;  // Re-throw to allow caller to handle
            }
        }

        /// <summary>
        /// Logs an error for missing Mandrill credentials
        /// </summary>
        private static async Task LogMissingCredentialsError(ApplicationDBContext context)
        {
            await ErrorLogsBLL.Add(context, new ErrorLogs
            {
                Description = "Email sending failed - Mandrill credentials invalid",
                Url = "EmailProcess",
                StackTrace = "Mandrill credentials missing"
            });
        }
    }
}