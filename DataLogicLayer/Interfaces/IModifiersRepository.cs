using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IModifiersRepository
{
    public IEnumerable<Modifiergroup> GetAllModifierGroup();

    public Task<ModifierGroupViewModel> GetModifierItemByIdAsync(long modifierId);
    public Task<List<ItemModifierGroupListViewModel>> GetModifierItemByItemId(long itemId);

    public Task<(IEnumerable<ModifierItemViewModel> modifierItems, int totalRecords)> GetModifierItemAsync(long modifierGroupId, int pageNo, int pageSize, string search);
    public Task<(IEnumerable<ModifierItemViewModel> modifierItems, int totalRecords)> GetAllModifierItemAsync(int pageNo, int pageSize, string search);


    public Task<string> AddModifierAsync(ModifierGroupViewModel model, long userId);
    public Task<string> EditModifierAsync(ModifierGroupViewModel model, long userId);

    public Task<ModifierGroupViewModel> GetModifierGroupByIdAsync(long modifierId);

    public Task<bool> DeleteModifierGroupAsync(long modifierGroupId, long userId);

    public Task<string> AddModifierItemAsync(AddEditModifierViewModel model, long userId);

    public Task<AddEditModifierViewModel> GetModifierByIdAsync(long modifierId);
}
