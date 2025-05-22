using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly IMemoryCache _cache;
    private readonly string _policyName;
    private readonly int _maxRequests;
    private readonly int _timeWindowSeconds;

    // Change constructor to get IMemoryCache via DI
    public RateLimitAttribute(IMemoryCache cache, string policyName, int maxRequests, int timeWindowSeconds)
    {
        _cache = cache;
        _policyName = policyName;
        _maxRequests = maxRequests;
        _timeWindowSeconds = timeWindowSeconds;
    }

    // Rest of your implementation...
}