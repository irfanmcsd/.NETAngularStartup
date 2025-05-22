using DevCodeArchitect.Localize;
using Microsoft.Extensions.Localization;

/// <summary>
/// Provides localized string resources from the GeneralResource files.
/// This service acts as a wrapper around IStringLocalizer for easier consumption.
/// </summary>
public class ResourceService
{
    private readonly IStringLocalizer<GeneralResource> _localizer;

    /// <summary>
    /// Initializes a new instance of the ResourceService.
    /// </summary>
    /// <param name="localizer">The string localizer for GeneralResource.</param>
    /// <exception cref="ArgumentNullException">Thrown when localizer is null.</exception>
    public ResourceService(IStringLocalizer<GeneralResource> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The resource key to look up.</param>
    /// <returns>The localized string if found, or the key name if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    /// <remarks>
    /// The returned value will be:
    /// - The localized string if the key exists
    /// - The key name itself if the key doesn't exist (standard IStringLocalizer behavior)
    /// - Empty string if the key exists but has an empty value
    /// </remarks>
    public string GetValue(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Resource key cannot be null or empty", nameof(key));
        }

        return _localizer[key];
    }
}