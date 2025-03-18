using System;

namespace DataLogicLayer.ViewModels;

public class UserViewModel
{
    public long CurrentUserId { get; set; }
    public IEnumerable<UserListViewModel>? UserList { get; set; }
    public PaginationViewModel? Page { get; set; }
}