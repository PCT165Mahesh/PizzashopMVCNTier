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


    RolePermissionsRepository(PizzaShopDbContext context)
    {
        _context = context;

    }
    public List<PermissionsViewModel> GetRoleAndPermissions(long roleId)
    {
        var permissions =  _context.Rolesandpermissions.Include(rp => rp.Permissionid).Where(rp => rp.Roleid == roleId).ToList();
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
