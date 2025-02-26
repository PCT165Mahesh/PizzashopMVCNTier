using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class UserRecordsRepository : IGetUserRecordsRepository
{
    private readonly PizzaShopDbContext _context;
    public UserRecordsRepository(PizzaShopDbContext context)
    {
        _context = context;
    }


    /*-----------------------------------------------------------------------------------------------Get User Records based on Search, Page no Method Implementation
    --------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region  Get User Records for Pagination
   public async Task<PaginationViewModel> GetAllUserRecordsAsync(int pageNo, int pageSize, string search)
    {
        var query = from u in _context.Users
                    join r in _context.Roles on u.Roleid equals r.RoleId
                    where u.Isdeleted == false &&
                        (string.IsNullOrEmpty(search) ||
                        EF.Functions.ILike(u.Firstname, $"%{search}%") ||
                        EF.Functions.ILike(u.Lastname, $"%{search}%") ||
                        EF.Functions.ILike(u.Email, $"%{search}%") ||
                        EF.Functions.ILike(r.Rolename, $"%{search}%")
                        )
                    select new UserListViewModel
                    {
                        UserId = u.Id,
                        FirstName = u.Firstname,
                        LastName = u.Lastname,
                        Email = u.Email,
                        Phone = u.Phone,
                        Status = u.Status,
                        RoleName = r.Rolename,
                        ImgUrl = u.Imgurl
                    };

        var totalRecords = await query.CountAsync();
        var users = await query.OrderBy(u => u.FirstName)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return new PaginationViewModel { UserList = users, TotalRecords = totalRecords };
    }
    #endregion

}
