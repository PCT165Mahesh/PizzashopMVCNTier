using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly PizzaShopDbContext _context;


    public RoleRepository(PizzaShopDbContext context)
    {
        _context = context;

    }
    public async Task<Role> GetRoleById(long roleId)
    {
        return await _context.Roles.Where(u => u.RoleId == roleId).FirstOrDefaultAsync();
    }

}
