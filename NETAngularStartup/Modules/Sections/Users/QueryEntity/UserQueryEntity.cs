using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity;

/// <summary>
/// Represents a query entity for searching and filtering users with various criteria
/// </summary>
/// <remarks>
/// Inherits from ContentEntity to provide common content-related properties.
/// Includes filters for user roles, locations, company associations, and authentication.
/// </remarks>
public class UserQueryEntity : ContentEntity
{
    #region Company Filters

    /// <summary>
    /// Gets or sets the company slug for filtering users by company
    /// </summary>
    [JsonProperty("companySlug")]
    [StringLength(100, ErrorMessage = "Company slug cannot exceed 100 characters")]
    public string? CompanySlug { get; set; }

    /// <summary>
    /// Gets or sets the company ID for filtering users by specific company
    /// </summary>
    [JsonProperty("companyId")]
    public int CompanyId { get; set; }

    #endregion

    #region User Type Filters

    /// <summary>
    /// Gets or sets the agent type filter
    /// </summary>
    /// <remarks>
    /// Defaults to All agent types
    /// </remarks>
    [JsonProperty("agentType")]
    public UserEnum.AgentTypes AgentType { get; set; } = UserEnum.AgentTypes.All;

    /// <summary>
    /// Gets or sets the action type filter (management vs non-management)
    /// </summary>
    /// <remarks>
    /// Defaults to NonManagement
    /// </remarks>
    [JsonProperty("actionType")]
    public UserEnum.ActionType ActionType { get; set; } = UserEnum.ActionType.NonManagement;

    #endregion

    #region Location Filters

    /// <summary>
    /// Gets or sets the user ID for specific user lookup
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the location ID for filtering users by location
    /// </summary>
    [JsonProperty("locationId")]
    public int LocationId { get; set; }

    /// <summary>
    /// Gets or sets the country filter
    /// </summary>
    [JsonProperty("country")]
    [StringLength(50, ErrorMessage = "Country name cannot exceed 50 characters")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the state/province filter
    /// </summary>
    [JsonProperty("state")]
    [StringLength(50, ErrorMessage = "State name cannot exceed 50 characters")]
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the city filter
    /// </summary>
    [JsonProperty("city")]
    [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the postal/zip code filter
    /// </summary>
    [JsonProperty("zip")]
    [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
    public string? Zip { get; set; }

    #endregion

    #region Role Filters

    /// <summary>
    /// Gets or sets the role name filter (e.g., Admin, Employer)
    /// </summary>
    [JsonProperty("userRoleName")]
    [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
    public string? UserRoleName { get; set; }

    /// <summary>
    /// Gets or sets the role ID filter
    /// </summary>
    [JsonProperty("userRoleId")]
    public string? UserRoleId { get; set; }

    #endregion

    #region Status Filters

    /// <summary>
    /// Gets or sets the email confirmation status filter
    /// </summary>
    /// <remarks>
    /// Defaults to Enabled (only users with confirmed emails)
    /// </remarks>
    [JsonProperty("emailConfirmed")]
    public Types.ActionTypes EmailConfirmed { get; set; } = Types.ActionTypes.Enabled;

    /// <summary>
    /// Gets or sets the subscription expiry option filter
    /// </summary>
    /// <remarks>
    /// Defaults to All (no filtering by subscription status)
    /// </remarks>
    [JsonProperty("subscriptionExpiryOption")]
    public Types.ExpiryOptions SubscriptionExpiryOption { get; set; } = Types.ExpiryOptions.All;

    /// <summary>
    /// Gets or sets the featured status filter
    /// </summary>
    /// <remarks>
    /// Defaults to All (no filtering by featured status)
    /// </remarks>
    [JsonProperty("isFeatured")]
    public Types.FeaturedTypes IsFeatured { get; set; } = Types.FeaturedTypes.All;

    #endregion

    #region Search Options

    /// <summary>
    /// Gets or sets the starting character for alphabetical user grouping
    /// </summary>
    /// <example>
    /// If set to "A", would return users whose names start with A
    /// </example>
    [JsonProperty("character")]
    public string? Character { get; set; }

    /// <summary>
    /// Gets or sets the grouping option for chart data
    /// </summary>
    /// <remarks>
    /// Defaults to None (no special grouping)
    /// </remarks>
    [JsonProperty("groupBy")]
    public Types.ChartGroupBy GroupBy { get; set; } = Types.ChartGroupBy.None;

    #endregion

    #region Authentication Fields

    /// <summary>
    /// Gets or sets the email address for authentication
    /// </summary>
    [JsonProperty("email")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication
    /// </summary>
    [JsonProperty("password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    #endregion
}