// SecurityHeadersMiddleware.cs
using Microsoft.Extensions.Options;

namespace DevCodeArchitect.Utilities;
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersConfig _config;

    public SecurityHeadersMiddleware(RequestDelegate next, IOptions<SecurityHeadersConfig> config)
    {
        _next = next;
        _config = config.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var response = context.Response;
        var headers = response.Headers;

        // Add Strict Transport Security
        if (!string.IsNullOrEmpty(_config.StrictTransportSecurity))
        {
            headers["Strict-Transport-Security"] = _config.StrictTransportSecurity;
        }

        // Add Content Security Policy
        if (!string.IsNullOrEmpty(_config.ContentSecurityPolicy))
        {
            headers["Content-Security-Policy"] = _config.ContentSecurityPolicy;
        }

        // Add X-Content-Type-Options
        if (!string.IsNullOrEmpty(_config.XContentTypeOptions))
        {
            headers["X-Content-Type-Options"] = _config.XContentTypeOptions;
        }

        // Add Referrer Policy
        if (!string.IsNullOrEmpty(_config.ReferrerPolicy))
        {
            headers["Referrer-Policy"] = _config.ReferrerPolicy;
        }

        await _next(context);
    }
}

// SecurityHeadersConfig.cs
public class SecurityHeadersConfig
{
    public string StrictTransportSecurity { get; set; }
    public string ContentSecurityPolicy { get; set; }
    public string XContentTypeOptions { get; set; }
    public string ReferrerPolicy { get; set; }
}