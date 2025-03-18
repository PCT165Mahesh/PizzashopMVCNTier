using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Interfaces;

public interface ITaxesAndFeesService
{
    public Task<TaxViewModel> GetTaxDetails(int pageNo, int pageSize, string search);
    public Task<TaxListViewModel> GetTaxById(long taxId);
    public Task<string> AddTax(TaxListViewModel model, long userId);
    public Task<string> EditTax(TaxListViewModel model, long userId);
    public Task<bool> DeleteTax(long taxid, string userName);
}
