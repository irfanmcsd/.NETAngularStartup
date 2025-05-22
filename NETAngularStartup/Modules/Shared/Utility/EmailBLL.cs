using System.Text.RegularExpressions;
using System.Net.Mail;
using DevCodeArchitect.DBContext;


namespace DevCodeArchitect.Utilities
{
    public class EmailBLLC
    {
        public static async Task SendMailMessage(ApplicationDBContext context, string? from, string? fromdisplayname, string? recepient, string? bcc, string? cc, string subject, string body)
        {
            // check if mandrill email is enabled
            if (!string.IsNullOrEmpty(EmailSettings.Mandril.ApiKey))
            {
                var emails = new List<string>();
                emails.Add(recepient);
                await Mandrill.EmailProcess.SendMail(context, from, fromdisplayname, emails, subject, body);
            }
            else
            {
                await EmailProcess.SendMailMessage(context, from, fromdisplayname, recepient, bcc, cc, subject, body);
            }
            /*else
            {
                // normal smtp mail processing
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

                    SmtpClient mSmtpClient = new SmtpClient();

                    mSmtpClient.Send(mMailMessage);
                }
            }*/
        }
    }
}
