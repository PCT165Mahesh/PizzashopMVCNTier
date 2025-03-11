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

    public async Task<(IEnumerable<ModifierItemViewModel> modifierItems, int totalRecords)> GetAllModifierItemAsync(int pageNo, int pageSize, string search)
    {
        IQueryable<ModifierItemViewModel> query = _context.Modifieritems
                                 .Include(m => m.Unit)
                                 .Where(m =>!m.Isdeleted)
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

    public async Task<ModifierGroupViewModel> GetModifierItemByIdAsync(long modifierId)
    {
        ModifierGroupViewModel? modifiergroup = await _context.Modifiergroups.Where(m => m.Id == modifierId).
                                    Include(m => m.Modifieritems).
                                    Select( m => new ModifierGroupViewModel {
                                        ModifierId = m.Id,
                                        Name = m.Name,
                                        Description = m.Description,
                                        ModifierItemList = m.Modifieritems.Select( i => new ModifierItemViewModel{
                                            ModifierItemId = i.Id,
                                            Name = i.Name,
                                            Rate = i.Rate,
                                            Quantity = i.Quantity,
                                        }).ToList()
                                    }).FirstOrDefaultAsync();

        return modifiergroup;
    }

    public async Task<List<ItemModifierGroupListViewModel>> GetModifierItemByItemId(long itemId)
    {
       List<ItemModifierGroupListViewModel> model =await _context.Itemmodifiergroups.Include(i => i.ModifierGroup.Modifieritems).
       Where(i => i.Itemid == itemId).Select(i => new ItemModifierGroupListViewModel{
        ItemId = i.Itemid,
        ModifierGroupId = i.ModifierGroupId,
        Name = i.ModifierGroup.Name,
        MinAllowed = i.MinAllowed,
        MaxAllowed = i.MaxAllowed,
        ModifierItemList = i.ModifierGroup.Modifieritems.Select( i => new ModifierItemViewModel{
            ModifierItemId = i.Id,
            Name = i.Name,
            Rate = i.Rate,
            Quantity = i.Quantity
        }).ToList()
       }).ToListAsync();


       return model;
    }

    public async Task<string> AddModifierAsync(ModifierGroupViewModel model, long userId)
    {
        Modifiergroup existingGroup = await _context.Modifiergroups.Where(m => m.Name == model.Name && !m.Isdeleted && m.Id != model.ModifierId).FirstOrDefaultAsync();
        if (existingGroup != null && existingGroup.Isdeleted == false)
        {
            return $"{model.Name} Item already exist! ";
        }
        if (existingGroup != null && existingGroup.Isdeleted == true)
        {
            existingGroup.Name = string.Concat(existingGroup.Name, DateTime.Now);
            _context.Modifiergroups.Update(existingGroup);
            await _context.SaveChangesAsync();
        }

        try
        {
            Modifiergroup modifiergroup = new Modifiergroup
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            _context.Modifiergroups.Add(modifiergroup);
            await _context.SaveChangesAsync();

            if(await AddModifierItem(modifiergroup.Id, model.ModifierItemList, userId))
            {
                return "true";
            }
            return "Failed To Add Item";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error In Category Repository :", ex.Message);
            return "Error Adding Item";
        }
    }

    public Task<string> EditModifierAsync(ModifierGroupViewModel model, long userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddModifierItem(long modifierGroupId, List<ModifierItemViewModel> ModifierItemList, long userId)
    {

        try
        {
            foreach(var item in ModifierItemList)
            {
                Modifiergroupitemmap? existingOne = await _context.Modifiergroupitemmaps.Where(mi=> mi.ModifierGroupId == modifierGroupId && mi.ModifierItemId == item.ModifierItemId && !mi.Isdeleted).FirstOrDefaultAsync();
                if(existingOne != null)
                {
                   continue;
                }
                Modifiergroupitemmap newModifierItemMap = new Modifiergroupitemmap
                {
                    ModifierGroupId = modifierGroupId,
                    ModifierItemId = item.ModifierItemId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };

                await _context.Modifiergroupitemmaps.AddAsync(newModifierItemMap);
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Modifier Item", ex.Message);
            return false;
        }
    }

    public async Task<ModifierGroupViewModel> GetModifierGroupByIdAsync(long modifierId)
    {
       ModifierGroupViewModel? model = await _context.Modifiergroups
                                        .Include(mg => mg.Modifiergroupitemmaps)
                                        .ThenInclude(mi => mi.ModifierItem)
                                        .Where(mg => mg.Id == modifierId && !mg.Isdeleted)
                                        .Select(mg => new ModifierGroupViewModel
                                        {
                                            ModifierId = mg.Id,
                                            Name = mg.Name,
                                            Description = mg.Description,
                                            ModifierItemList = mg.Modifiergroupitemmaps
                                            .Where(m => m.Id == modifierId && !m.Isdeleted)
                                            .Select(m => new ModifierItemViewModel
                                            {
                                                ModifierItemId = m.ModifierItem.Id,
                                                Name = m.ModifierItem.Name,
                                                Unit = m.ModifierItem.Unit.Name,
                                                Rate = m.ModifierItem.Rate,
                                                Quantity = m.ModifierItem.Quantity,
                                            }).ToList()
                                        }).FirstOrDefaultAsync();


        return model;
    }
}


