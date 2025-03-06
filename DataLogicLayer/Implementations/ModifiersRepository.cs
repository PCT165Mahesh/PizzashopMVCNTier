using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class ModifiersRepository : IModifiersRepository
{
    private readonly PizzaShopDbContext _context;


    public ModifiersRepository(PizzaShopDbContext context)
    {
        _context = context;

    }
    public IEnumerable<Modifiergroup> GetAllModifierGroup()
    {
        return _context.Modifiergroups.Where(m => !m.Isdeleted).ToList();
    }

    public async Task<(IEnumerable<ModifierItemViewModel> modifierItems, int totalRecords)> GetModifierItemAsync(long modifierGroupId, int pageNo, int pageSize, string search)
    {
        IQueryable<ModifierItemViewModel> query = _context.Modifieritems
                                 .Include(m => m.Unit)
                                 .Where(m => m.ModifierGroupId == modifierGroupId && !m.Isdeleted)
                                 .Select(m => new ModifierItemViewModel
                                 {
                                     ModifierItemId = m.Id,
                                     Name = m.Name,
                                     Rate = m.Rate,
                                     Unit = m.Unit.Name,
                                     Quantity = m.Quantity,
                                 });

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(search) ||
                                m.Rate.ToString().Contains(search) ||
                                m.Quantity.ToString().Contains(search));
        }

        int totalRecords = await query.CountAsync();

        IEnumerable<ModifierItemViewModel> modifierItems = await query
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

        return (modifierItems, totalRecords);
    }
}
