using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class RolePermissionsRepository : IRolePermissionsRepository
{
    private readonly PizzaShopDbContext _context;

    public RolePermissionsRepository(PizzaShopDbContext context)
    {
        _context = context;

    }

    public async Task<bool> EditPermission(RolePermissionViewModel model, long userId)
    {
       List<Rolesandpermission> permissions = await _context.Rolesandpermissions.Where(rp => rp.Roleid == model.RoleId).ToListAsync();
       if(permissions == null){
        return false;
       }

       foreach(var permList in model.Permissions){
        // Inner Loop for the rolePermisions tab;e
        foreach(var rolePerm in permissions){
            rolePerm.Permissionid = permList.PermissionId;
            rolePerm.Canview = permList.View;
            rolePerm.Canaddedit = permList.AddOrEdit;
            rolePerm.Candelete = permList.Delete;
            _context.Rolesandpermissions.Update(rolePerm);
            await _context.SaveChangesAsync();
        }
       }
        return true;
    }


    public List<PermissionsViewModel> GetRoleAndPermissions(long roleId)
    {
        var permissions =  _context.Rolesandpermissions.Where(rp => rp.Roleid == roleId).Include(rp => rp.Permission).ToList();
        List<PermissionsViewModel> model = new List<PermissionsViewModel>();
        
        foreach(var perm in permissions){
            model.Add(
                new PermissionsViewModel(){
                    PermissionId = perm.Permissionid,
                    PermissionName = perm.Permission.Name,
                    View = perm.Canview,
                    AddOrEdit = perm.Canaddedit,
                    Delete = perm.Candelete
                }
            );
        }
        return model;
    }
}
