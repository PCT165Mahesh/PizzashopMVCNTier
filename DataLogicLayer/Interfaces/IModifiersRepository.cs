using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface IModifiersRepository
{
    public IEnumerable<Modifiergroup> GetAllModifierGroup();

    public Task<(IEnumerable<ModifierItemViewModel> modifierItems, int totalRecords)> GetModifierItemAsync(long modifierGroupId, int pageNo, int pageSize, string search);
}
