using DataLogicLayer.Models;

namespace DataLogicLayer.Interfaces;

public interface IRoleRepository
{
    public Task<Role> GetRoleById(long roleId);
}
