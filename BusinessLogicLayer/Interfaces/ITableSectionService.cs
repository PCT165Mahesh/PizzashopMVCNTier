using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Implementations;

public interface ITableSectionService
{
    #region Section CRUD
    public Task<IEnumerable<SectionViewModel>> GetSectionsList();
    public Task<SectionViewModel> GetSectionById(long sectionId);
    public Task<string> AddSection(SectionViewModel model, long userId);
    public Task<string> EditSection(SectionViewModel model, long userId);
    public Task<bool> DeleteSection(long id, string userName);
    #endregion

    #region Table CRUD
    public Task<TableListViewModel> GetTableList(long sectionId,int pageNo, int pageSize, string search);
    public Task<TableViewModel> GetTableById(long tableId);

    public Task<string> AddTable(TableViewModel model, long userId);
    public Task<string> EditTable(TableViewModel model, long userId);
    public Task<bool> DeleteTable(long sectionId,long id, string userName);
    public Task<bool> DeleteSelectedTable(List<long> id, long sectionId, string userName);

    #endregion
}
