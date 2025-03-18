using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface ITaxesAndFeesRepository
{
    public Task<(List<TaxListViewModel> taxList, int totalRecords)> GetAllTaxDetailsAsync(int pageNo, int pageSize, string search);
    public Task<Taxis> GetTaxByIdAsync(long taxId);
    public Task<string> AddTaxAsync(TaxListViewModel model, long userId);
    public Task<string> EditTaxAsync(TaxListViewModel model, long userId);
    public Task<bool> DeleteTaxAsync(long taxId, long userId);
}
