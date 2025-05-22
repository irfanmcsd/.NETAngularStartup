using DevCodeArchitect.DBContext;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using DevCodeArchitect.Entity;

namespace DevCodeArchitect.Identity
{
    /// <summary>
    /// Extended application user with additional properties beyond basic IdentityUser
    /// </summary>
    /// <remarks>
    /// Inherits from IdentityUser to provide core user management functionality while
    /// adding custom properties specific to the application's requirements.
    /// Includes user profile information, status flags, and navigation properties.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the URL-friendly version of the username
        /// </summary>
        /// <example>"john-doe"</example>
        [JsonProperty("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// Gets or sets the user's first name
        /// </summary>
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name
        /// </summary>
        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user account was created
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the path or URL to the user's avatar image
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Gets or sets whether the user account is enabled (1) or disabled (0)
        /// </summary>
        [JsonProperty("isEnabled")]
        public Types.ActionTypes IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the user is marked as featured (1) or not (0)
        /// </summary>
        [JsonProperty("isFeatured")]
        public Types.FeaturedTypes IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets the user's primary role name
        /// </summary>
        [JsonProperty("userRole")]
        public string? UserRole { get; set; }

        /// <summary>
        /// Gets or sets the user type identifier
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// 0 = Regular user
        /// 1 = Service provider
        /// 2 = Administrator
        /// etc.
        /// </remarks>
        public UserEnum.UserTypes Type { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Gets or sets a transient status message for UI feedback (not persisted in database)
        /// </summary>
        [NotMapped]
        [JsonProperty("actionStatus")]
        public string? ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the user's profile URL (not persisted in database)
        /// </summary>
        [NotMapped]
        [JsonProperty("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the collection of blogs associated with this user
        /// </summary>
        /// <remarks>
        /// Represents a one-to-many relationship between users and blogs.
        /// </remarks>
        [JsonProperty("blogs")]
        public ICollection<Blogs>? Blogs { get; set; }
    }
}
