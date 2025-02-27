using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Interfaces;

public interface IRolePermissionService
{
    public RolePermissionViewModel GetRolePermissions(long roleId);
}
