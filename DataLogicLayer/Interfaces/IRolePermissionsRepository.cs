using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IRolePermissionsRepository
{
    public List<PermissionsViewModel> GetRoleAndPermissions(long roleId);

    public Task<bool> EditPermission(RolePermissionViewModel model, long userId);
}
