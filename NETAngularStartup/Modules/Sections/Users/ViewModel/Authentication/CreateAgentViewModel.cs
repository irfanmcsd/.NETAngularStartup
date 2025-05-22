using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevCodeArchitect.Entity;

/// <summary>
/// ViewModel for agent account creation, extending the base account creation with agent-specific properties
/// </summary>
/// <remarks>
/// Inherits from <see cref="CreateAccountViewModel"/> to include all basic account creation fields
/// while adding agent-specific requirements. Used in agent registration workflows.
/// </remarks>
public class CreateAgentViewModel : CreateAccountViewModel
{
    /// <summary>
    /// Gets or sets the agent type (required)
    /// </summary>
    /// <remarks>
    /// Determines the agent's classification within the system. Possible values are defined
    /// in <see cref="UserEnum.UserTypes"/>.
    /// </remarks>
    /// <example>1</example>
    [Required(ErrorMessage = "Agent type is required")]
    [Display(Name = "Agent Type", Description = "The classification type of the agent")]
    [DefaultValue(UserEnum.UserTypes.NormalUser)] // Set default value if appropriate
    public UserEnum.UserTypes Type { get; set; } = UserEnum.UserTypes.NormalUser;
}