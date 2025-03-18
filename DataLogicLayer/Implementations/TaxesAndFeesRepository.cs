using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class TaxesAndFeesRepository : ITaxesAndFeesRepository
{
    private readonly PizzaShopDbContext _context;


    public TaxesAndFeesRepository(PizzaShopDbContext context)
    {
        _context = context;

    }

    #region Taxes CRUD

    public async Task<(List<TaxListViewModel> taxList, int totalRecords)> GetAllTaxDetailsAsync(int pageNo, int pageSize, string search)
    {
        IQueryable<TaxListViewModel> query = _context.Taxes
                                            .Where(t => !t.Isdeleted)
                                            .Select(t => new TaxListViewModel
                                            {
                                                TaxId = t.Id,
                                                TaxName = t.Name,
                                                TaxValue = t.Amount,
                                                TaxType = t.Taxtype,
                                                Isenabled = t.Isenabled,
                                                Default = (bool)t.DefaultTax
                                            }).OrderBy(t => t.TaxId);

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(u => u.TaxName.ToLower().Contains(search) ||
                                u.TaxValue.ToString().Contains(search));
        }


        int totalRecords = await query.CountAsync();
        List<TaxListViewModel> taxList = await query
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return (taxList, totalRecords);
    }

    public async Task<Taxis> GetTaxByIdAsync(long taxId)
    {
        return await _context.Taxes.Where(t => t.Id == taxId).FirstOrDefaultAsync();
    }

    public async Task<string> AddTaxAsync(TaxListViewModel model, long userId)
    {
        Taxis? existingTax = await _context.Taxes.Where(t => t.Name == model.TaxName && t.Id != model.TaxId && !t.Isdeleted).FirstOrDefaultAsync();
        if (existingTax != null && existingTax.Isdeleted == false)
        {
            return $"{model.TaxName} Tax already exist! ";
        }
        if (existingTax != null && existingTax.Isdeleted == true)
        {
            existingTax.Name = string.Concat(existingTax.Name, DateTime.Now);
            _context.Taxes.Update(existingTax);
            await _context.SaveChangesAsync();
        }

        try
        {
            Taxis newTax = new Taxis
            {
                Name = model.TaxName,
                Taxtype = model.TaxType,
                Amount = model.TaxValue,
                Isenabled = model.Isenabled,
                DefaultTax = model.Default,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
            };

            await _context.Taxes.AddAsync(newTax);
            await _context.SaveChangesAsync();
            return "true";

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error In Taxes and Fees Repository :", ex.Message);
            return "Failed To Add Tax";
        }
    }

    public async Task<string> EditTaxAsync(TaxListViewModel model, long userId)
    {
        try
        {
            Taxis? existingTax = await _context.Taxes.Where(t => t.Name == model.TaxName && t.Id != model.TaxId && !t.Isdeleted).FirstOrDefaultAsync();
            if (existingTax != null && existingTax.Isdeleted == false)
            {
                return $"{model.TaxName} Tax already exist! ";
            }
            if (existingTax != null && existingTax.Isdeleted == true)
            {
                existingTax.Name = string.Concat(existingTax.Name, DateTime.Now);
                _context.Taxes.Update(existingTax);
                await _context.SaveChangesAsync();
            }

            Taxis? tax = await _context.Taxes.Where(t => t.Id == model.TaxId && !t.Isdeleted).FirstOrDefaultAsync();

            if (tax == null)
            {
                return "Tax Does not exists";
            }

            tax.Name = model.TaxName;
            tax.Taxtype = model.TaxType;
            tax.Amount = model.TaxValue;
            tax.Isenabled = model.Isenabled;
            tax.DefaultTax = model.Default;
            tax.UpdatedBy = userId;
            tax.UpdatedAt = DateTime.Now;
            _context.Taxes.Update(tax);
            await _context.SaveChangesAsync();
            return "true";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Updating Tax", ex.Message);
            return "Failed To Updated Tax";
        }
    }

    public async Task<bool> DeleteTaxAsync(long taxId, long userId)
    {
        try
        {
            Taxis? tax = await _context.Taxes.Where(t => t.Id == taxId && !t.Isdeleted).FirstOrDefaultAsync();

            if(tax == null) return false;

            tax.Isdeleted = true;
            tax.UpdatedAt = DateTime.Now;
            tax.UpdatedBy = userId;
            _context.Taxes.Update(tax);
            await _context.SaveChangesAsync();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error In Delete tax Repository: ", ex.Message);
            return false;
        }
    }
    #endregion
}
