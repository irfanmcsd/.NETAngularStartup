using System.Resources;
using System.Globalization;

namespace DevCodeArchitect.Services;

public enum EmailTemplateType
{
    EmailConfirmation,
    EmailConfirmed,
    PasswordReset,
    PasswordResetConfirmed,
    AccountLocked
}

public class EmailTemplateService
{
    private readonly ResourceManager _resourceManager;
    private readonly IConfiguration _config;

    public EmailTemplateService(IConfiguration config)
    {
        _resourceManager = new ResourceManager(
            "MagicTradeBot.Resources.EmailTemplates",
            typeof(EmailTemplateService).Assembly);
        _config = config;
    }

    // Update TemplateService to accept enum
    public string GetTemplate(EmailTemplateType templateType, CultureInfo culture = null)
    {
        return _resourceManager.GetString(templateType.ToString(), culture)
               ?? throw new ArgumentException($"Template '{templateType}' not found");
    }

    /*public string GetTemplate(string templateName, CultureInfo culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return _resourceManager.GetString(templateName, culture)
               ?? throw new ArgumentException($"Template '{templateName}' not found");
    }*/

    public string RenderTemplate(EmailTemplateType templateType, Dictionary<string, string> replacements,
                               CultureInfo culture = null)
    {
        var template = GetTemplate(templateType, culture);

        foreach (var replacement in replacements)
        {
            template = template.Replace(
                $"{{{replacement.Key}}}",
                replacement.Value);
        }

        // Add config-based replacements
        template = template
            .Replace("{PageCaption}", _config["SiteConfig:PageCaption"])
            .Replace("{SupportEmail}", _config["SiteConfig:SupportEmail"]);

        return template;
    }
}