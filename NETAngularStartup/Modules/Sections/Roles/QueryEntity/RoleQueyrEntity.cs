namespace DevCodeArchitect.Entity;
public class RoleQueryEntity : ContentEntity
{
    public string? RoleID { get; set; }
    public string? RoleName { get; set; }
    public RoleEnum.Types Type { get; set; } = RoleEnum.Types.Admin;
}
