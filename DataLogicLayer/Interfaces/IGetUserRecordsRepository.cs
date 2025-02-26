using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IGetUserRecordsRepository
{
    public Task<PaginationViewModel> GetAllUserRecordsAsync(int pageNo = 1, int pageSize = 3, string search = "");
}
