using System;
using System.Text.RegularExpressions;
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Utilities;

/// <summary>
/// Utility class for processing mails.
/// </summary>
namespace DevCodeArchitect.Utilities
{
    public class MailProcess
    {
        public static string Process2(string? text, string? keyword, string? value)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(value))
                return Regex.Replace(text, keyword, value);
            else
                return "";
        }

        public static async Task Send_Mail(ApplicationDBContext context, string emailaddress, string subject, string content)
        {
            //// Sender Address
            var fromEmail = EmailSettings.FromAddress;
            var fromEmailDisplayName = EmailSettings.FromName;
            if (fromEmail == "")
                return;

            try
            {
                if (EmailSettings.Enabled)
                {
                    await EmailBLLC.SendMailMessage(context, fromEmail, fromEmailDisplayName, emailaddress, null, null, subject, content);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}

