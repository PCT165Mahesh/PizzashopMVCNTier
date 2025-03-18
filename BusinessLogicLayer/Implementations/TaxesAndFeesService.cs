using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Implementations;

public class TaxesAndFeesService : ITaxesAndFeesService
{
    private readonly ITaxesAndFeesRepository _taxesAndFeesRepository;
    private readonly IUserRepository _userRepository;

    public TaxesAndFeesService(ITaxesAndFeesRepository taxesAndFeesRepository, IUserRepository userRepository)
    {
        _taxesAndFeesRepository = taxesAndFeesRepository;
        _userRepository = userRepository;
    }

    #region Taxes CRUD
    public async Task<TaxViewModel> GetTaxDetails(int pageNo, int pageSize, string search)
    {
        TaxViewModel model = new() { Page = new()};

        var taxData = await _taxesAndFeesRepository.GetAllTaxDetailsAsync(pageNo, pageSize, search);
        model.TaxList = taxData.taxList;
        model.Page.SetPagination(taxData.totalRecords,pageSize, pageNo);

        return model;
    }

    public async Task<TaxListViewModel> GetTaxById(long taxId)
    {
        Taxis tax = await _taxesAndFeesRepository.GetTaxByIdAsync(taxId);
        TaxListViewModel model = new TaxListViewModel{
            TaxId = tax.Id,
            TaxName = tax.Name,
            TaxType = tax.Taxtype,
            TaxValue = tax.Amount,
            Isenabled= tax.Isenabled,
            Default = (bool)tax.DefaultTax,
        };
        return model;
    }

    public async Task<string> AddTax(TaxListViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _taxesAndFeesRepository.AddTaxAsync(model, userId);
    }

    public async Task<string> EditTax(TaxListViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _taxesAndFeesRepository.EditTaxAsync(model, userId);
    }

    public async Task<bool> DeleteTax(long taxid, string userName)
    {
        if(taxid == 0)
        {
            return false;
        }

        User user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        return await _taxesAndFeesRepository.DeleteTaxAsync(taxid, user.Id);
    }
    #endregion
}
