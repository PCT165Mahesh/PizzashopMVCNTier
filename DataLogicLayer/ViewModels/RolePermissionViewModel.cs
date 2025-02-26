namespace DataLogicLayer.ViewModels;

public class RolePermissionViewModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }

    public List<PermissionsViewModel> Permissions { get; set; } = new List<PermissionsViewModel>();
}
