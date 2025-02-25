using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IGetUserRecordsRepository
{
    public (List<UserListViewModel> Users, int totalRecords) GetAllUserRecords(int pageNo = 1, int pageSize = 3, string search = "");
}
