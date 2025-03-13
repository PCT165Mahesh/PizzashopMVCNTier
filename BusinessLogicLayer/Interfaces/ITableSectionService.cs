using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Implementations;

public interface ITableSectionService
{
    public Task<IEnumerable<SectionViewModel>> GetSectionsList();
    public Task<TableListViewModel> GetTableList(long sectionId,int pageNo, int pageSize, string search);

}
