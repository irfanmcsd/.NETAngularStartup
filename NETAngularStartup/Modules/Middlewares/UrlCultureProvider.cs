using DevCodeArchitect.Utilities;
using Microsoft.AspNetCore.Localization;
using System.IO;
using System.Text.RegularExpressions;

public class UrlCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        // working code
        /*
        var culture = httpContext.Request.Query["culture"].ToString();
        if (!string.IsNullOrEmpty(culture))
        {
            // Validate culture
            var supportedCultures = new[] { "en", "fr", "de" };
            foreach(var c in supportedCultures)
            {
                if (c == culture)
                {
                    return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture));
                }
            }
        }*/
        var path = httpContext.Request.Path;
        if (path.HasValue && path.Value.Length >= 3)
        {
            var cultureSegment = path.Value.Split('/')[1]?.ToLower();

            if (!string.IsNullOrEmpty(cultureSegment))
            {                
                foreach (var c in CultureUtil.SupportedCultures())
                {
                    if (c == cultureSegment)
                    {
                        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(cultureSegment));
                    }
                }
               
            }
        }
        return NullProviderCultureResult;

    }
}