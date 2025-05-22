using DevCodeArchitect.Entity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevCodeArchitect.Identity
{
    /// <summary>
    /// Extended application role with additional properties beyond basic IdentityRole
    /// </summary>
    /// <remarks>
    /// Inherits from IdentityRole to provide core role management functionality while
    /// adding custom properties specific to the application's requirements.
    /// </remarks>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Gets or sets the description of the role
        /// </summary>
        /// <value>
        /// A human-readable explanation of the role's purpose and permissions
        /// </value>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the role was created
        /// </summary>
        /// <value>
        /// DateTime value with default of DateTime.MinValue if not set
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the IP address from which the role was created
        /// </summary>
        /// <value>
        /// String representation of the IPv4 or IPv6 address
        /// </value>
        public string? IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the role type identifier
        /// </summary>
        /// <value>
        /// Byte value representing the role type (e.g., 0=System, 1=Admin, 2=User)
        /// </value>
        public RoleEnum.Types Type { get; set; }

        /// <summary>
        /// Gets or sets a transient status message for UI feedback
        /// </summary>
        /// <remarks>
        /// Marked as [NotMapped] to exclude from database persistence.
        /// Used for temporary status messages during role operations.
        /// </remarks>
        [NotMapped]
        public string? ActionStatus { get; set; }
    }

    /// <summary>
    /// Junction entity for many-to-many relationship between users and roles
    /// </summary>
    /// <remarks>
    /// Inherits from IdentityUserRole to serve as the join table in EF Core
    /// for the user-role relationship. Can be extended with additional properties
    /// like assignment date or assignment reason if needed.
    /// </remarks>
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        // Additional properties can be added here if needed
        // Example:
        // public DateTime AssignedDate { get; set; }
        // public string AssignedBy { get; set; }
    }
}