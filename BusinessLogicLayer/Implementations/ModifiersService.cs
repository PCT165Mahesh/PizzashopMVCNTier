using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Implementations;

public class ModifiersService : IModifiersService
{
    private readonly IModifiersRepository _modifiersRepository;
    private readonly IUserRepository _userRepository;

    public ModifiersService(IModifiersRepository modifiersRepository, IUserRepository userRepository)
    {
        _modifiersRepository = modifiersRepository;
        _userRepository = userRepository;
    }

    /*---------------------------------------------------------------------------Modifier GROUP CRUD------------------------------------------------------------------------------*/

    #region Modifier Group CRUD
    public IEnumerable<ModifierGroupViewModel> GetAllModifierGroup()
    {
        IEnumerable<Modifiergroup> modifiergroups = _modifiersRepository.GetAllModifierGroup();
        return modifiergroups.Select(m => new ModifierGroupViewModel
        {
            ModifierId = m.Id,
            Name = m.Name,
            Description = m.Description
        });
    }
    public async Task<ModifierGroupViewModel> GetModifierGroupById(long modifierGroupId)
    {
        return await _modifiersRepository.GetModifierGroupByIdAsync(modifierGroupId);
    }

    // For Existing Modifiers In Edit Modifier Item
    public async Task<ModifierGroupViewModel> GetModifierItemById(long modifierGroupId)
    {
        return await _modifiersRepository.GetModifierItemByIdAsync(modifierGroupId);
    }

    // Item List for Existing Modifier In Add Modifier Item
    public async Task<ModifierItemListViewModel> GetAllModfierItems(int pageNo = 1, int pageSize = 3, string search = "")
    {
        ModifierItemListViewModel model = new() { Page = new() };
        var modifierItemData = await _modifiersRepository.GetAllModifierItemAsync(pageNo, pageSize, search);
        model.ModifierItemList = modifierItemData.modifierItems;
        model.Page.SetPagination(modifierItemData.totalRecords, pageSize, pageNo);
        return model;
    }

    public async Task<string> AddModifierGroup(ModifierGroupViewModel model, long userId)
    {
        if (model == null)
        {
            return "Model is Empty";
        }
        return await _modifiersRepository.AddModifierGroupAsync(model, userId);
    }

    public async Task<string> EditModifierGroup(ModifierGroupViewModel model, long userId)
    {
        if (model == null)
        {
            return "Model is Empty";
        }
        return await _modifiersRepository.EditModifierGroupAsync(model, userId);
    }

    public async Task<bool> DeleteModifierGroup(long modifierGroupId, string userName)
    {
        if (modifierGroupId == 0 || modifierGroupId == null)
        {
            return false;
        }

        User user = await _userRepository.GetUserByUserName(userName);
        if (user == null)
        {
            return false;
        }

        return await _modifiersRepository.DeleteModifierGroupAsync(modifierGroupId, user.Id);
    }

    #endregion

    /*---------------------------------------------------------------------------Modifier Items CRUD------------------------------------------------------------------------------*/

    #region Modifier Items CRUD
    public async Task<ModifierItemListViewModel> GetModfierItems(long modifierGroupId = 1, int pageNo = 1, int pageSize = 3, string search = "")
    {
        ModifierItemListViewModel model = new() { Page = new() };
        var modifierItemData = await _modifiersRepository.GetModifierItemAsync(modifierGroupId, pageNo, pageSize, search); ;

        model.ModifierItemList = modifierItemData.modifierItems;
        model.Page.SetPagination(modifierItemData.totalRecords, pageSize, pageNo);
        return model;
    }

    public async Task<AddEditModifierViewModel> GetModifierById(long modifierId,long modifierGroupId)
    {
        return await _modifiersRepository.GetModifierByIdAsync(modifierId, modifierGroupId);
    }

    public List<long> GetModifierGroupForModifierItem(long modifierId)
    {
        return _modifiersRepository.GetSelectedModifierGroupForModifer(modifierId);
    }

    public async Task<string> AddModifierItem(AddEditModifierViewModel model, long userId)
    {
        if (model == null)
        {
            return "Model is Empty";
        }
        return await _modifiersRepository.AddModifierItemAsync(model, userId);
    }
    public async Task<string> EditModifierItem(AddEditModifierViewModel model, long userId)
    {
        if (model == null)
        {
            return "Model is Empty";
        }
        return await _modifiersRepository.EditModifierItemAsync(model, userId);
    }

    public async Task<bool> DeleteModifierItem(long modifierGroupId,long modifierId, string userName)
    {
        if (modifierId == 0 || modifierId == null)
        {
            return false;
        }

        User user = await _userRepository.GetUserByUserName(userName);
        if (user == null)
        {
            return false;
        }

        return await _modifiersRepository.DeleteModifierItemAsync(modifierGroupId,modifierId, user.Id);
    }
    #endregion
    
    #region DELETE : Mass Delete Modifiers
    public async Task<bool> DeleteSelectedModifier(List<long> id, long modifierGroupId, string userName)
    {
        if(id.IsNullOrEmpty())
        {
            return false;
        }
        User user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        bool result = true;

        foreach(long modifierId in id)
        {
            result = await _modifiersRepository.DeleteModifierItemAsync(modifierGroupId,modifierId, user.Id);
            if(result == false)
                return result;
        }
        return result;
    }
    #endregion

    /*---------------------------------------------------------------------------Selected Modifier Items For Edit Item Modal------------------------------------------------------------------------------*/

    #region Selected Modifier Items For Edit Item Modal
    public Task<List<ItemModifierGroupListViewModel>> GetAllModifierItemById(long itemId)
    {
        return _modifiersRepository.GetModifierItemByItemId(itemId);
    }
    #endregion
}
