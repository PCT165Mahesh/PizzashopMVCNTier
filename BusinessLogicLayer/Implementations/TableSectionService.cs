using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Implementations;

public class TableSectionService : ITableSectionService
{
    private readonly ITableSectionRepository _tableSectionRepository;
    private readonly IUserRepository _userRepository;

    public TableSectionService(ITableSectionRepository tableSectionRepository, IUserRepository userRepository)
    {
        _tableSectionRepository = tableSectionRepository;
        _userRepository = userRepository;
    }

/*---------------------------------------------------------------------------Sections CRUD------------------------------------------------------------------------------*/

    #region Get Sections List
    public async Task<IEnumerable<SectionViewModel>> GetSectionsList()
    {
        IEnumerable<Section> sections = await _tableSectionRepository.GetSectionsListAsync();
        return sections.Select(s => new SectionViewModel{
            SectionID = s.SectionId,
            Name = s.Name,
            Description = s.Description
        });
    }

    public async Task<SectionViewModel> GetSectionById(long sectionId)
    {
        Section section = await _tableSectionRepository.GetSectionByIdAsync(sectionId);
        SectionViewModel model = new SectionViewModel{
            SectionID = section.SectionId,
            Name = section.Name,
            Description = section.Description
        };
        return model;
    }
    #endregion

    #region ADD : Section
    public async Task<string> AddSection(SectionViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _tableSectionRepository.AddSectionAsync(model, userId);
    }
    #endregion

    #region EDIT : Section

    public async Task<string> EditSection(SectionViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _tableSectionRepository.EditSectionAsync(model, userId);
    }
    #endregion

    #region DELETE : Section
    public async Task<bool> DeleteSection(long id, string userName)
    {
        if(id == 0)
        {
            return false;
        }

        User user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        return await _tableSectionRepository.DeleteSectionAsync(id, user.Id);
    }
    #endregion

/*---------------------------------------------------------------------------Table CRUD------------------------------------------------------------------------------*/


    #region Get Table List
    public async Task<TableListViewModel> GetTableList(long sectionId,int pageNo, int pageSize, string search)
    {
        TableListViewModel model = new() {Page = new ()};
        var tableData = await _tableSectionRepository.GetTableListAsync(sectionId, pageNo, pageSize, search);

        model.TableList = tableData.tables;
        model.Page.SetPagination(tableData.totalRecords, pageSize, pageNo);
        return model;
    }

    public async Task<TableViewModel> GetTableById(long tableId)
    {
        Table table = await _tableSectionRepository.GetTableByIdAsync(tableId);
        TableViewModel model = new TableViewModel{
            TableId = table.Id,
            IsOccupied = table.IsOccupied,
            SectionId = table.Sectionid,
            TableName = table.Name,
            Capacity = table.Capacity
        };
        return model;
    }
    #endregion

    #region ADD : Table
    public async Task<string> AddTable(TableViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _tableSectionRepository.AddTableAsync(model, userId);
    }
    #endregion
    
    #region EDIT : Table

    public async Task<string> EditTable(TableViewModel model, long userId)
    {
        if(model == null)
        {
            return "Model is Empty";
        }
        return await _tableSectionRepository.EditTableAsync(model, userId);
    }
    #endregion

    #region DELETE : Table
    public async Task<bool> DeleteTable(long sectionId,long id, string userName)
    {
        if(id == 0)
        {
            return false;
        }

        User user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        return await _tableSectionRepository.DeleteTableAsync(sectionId,id, user.Id);
    }
    #endregion

    #region DELETE : Mass Delete Tabled
    public async Task<bool> DeleteSelectedTable(List<long> id, long sectionId, string userName)
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

        foreach(long tableId in id)
        {
            result = await _tableSectionRepository.DeleteTableAsync(sectionId,tableId, user.Id);
            if(result == false)
                return result;
        }
        return result;
    }
    #endregion
}


