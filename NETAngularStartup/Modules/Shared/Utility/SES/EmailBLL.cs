
using DevCodeArchitect.DBContext;
using DevCodeArchitect.Entity;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DevCodeArchitect.Utilities;

public class EmailProcess
{
    /// <summary>
    /// Sending email through AWS Simple Email Service (SES)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="fromdisplayname"></param>
    /// <param name="recepient"></param>
    /// <param name="bcc"></param>
    /// <param name="cc"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    public static async Task SendMailMessage(ApplicationDBContext context, string from, string fromdisplayname, string recepient, string? bcc, string? cc, string subject, string body)
    {
        if (!string.IsNullOrEmpty(EmailSettings.Smtp.Host)
            && !string.IsNullOrEmpty(EmailSettings.Smtp.UserName)
            && !string.IsNullOrEmpty(EmailSettings.Smtp.Password))
        {
            string patternLenient = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex reLenient = new Regex(patternLenient);

            bool isLenientMatch = reLenient.IsMatch(recepient);
            // Instantiate a new instance of MailMessage
            MailMessage mMailMessage = new MailMessage();
            // Set the recepient address of the mail message
            mMailMessage.From = new MailAddress(from, fromdisplayname);
            // Set the recepient address of the mail message
            if (isLenientMatch)
            {
                mMailMessage.To.Add(new MailAddress(recepient));
                if (bcc != null)
                {
                    isLenientMatch = reLenient.IsMatch(bcc);
                    if (isLenientMatch)
                        mMailMessage.To.Add(new MailAddress(bcc));
                }
                if (cc != null)
                {
                    isLenientMatch = reLenient.IsMatch(cc);
                    if (isLenientMatch)
                        mMailMessage.To.Add(new MailAddress(cc));
                }

                mMailMessage.Subject = subject;

                mMailMessage.Body = body;

                mMailMessage.IsBodyHtml = true;

                mMailMessage.Priority = MailPriority.Normal;

                var smtpClient = new SmtpClient();
                smtpClient.Host = EmailSettings.Smtp.Host;
                smtpClient.Port = 25;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(EmailSettings.Smtp.UserName, EmailSettings.Smtp.Password);
                smtpClient.Send(mMailMessage);
            }
        }
        else
        {
            var errorObj = await ErrorLogsBLL.Add(context, new ErrorLogs()
            {
                Description = "Email Sending Failed - SES Credentials Invalid",
                Url = "Utility -> SES -> EmailBLL",
                StackTrace = "SES Credentials Missing"
            });
        }
    }
}
