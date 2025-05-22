namespace DevCodeArchitect.Utilities;

public class EmailSettings
{
    public static bool Enabled { get; set; }
    public static string Provider { get; set; } = "SMTP"; // [SMTP|SendGrid|SES]
    public static string SupportEmail { get; set; } = string.Empty;
    public static string FromAddress { get; set; } = string.Empty;
    public static string FromName { get; set; } = string.Empty;
    public static EmailSMTPSettings Smtp { get; set; } = new EmailSMTPSettings();
    public static EmailMandrilSettings Mandril { get; set; } = new EmailMandrilSettings();
}

public class EmailSMTPSettings
{
    public string Host { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set;} = string.Empty;
    public string Port { get; set; } = string.Empty;
    public bool EnableSSL { get; set; }
}

public class EmailMandrilSettings
{
    public string ApiKey { get; set; } = string.Empty;
}