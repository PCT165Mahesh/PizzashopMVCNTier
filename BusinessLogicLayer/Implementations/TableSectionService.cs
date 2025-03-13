using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace BusinessLogicLayer.Implementations;

public class TableSectionService : ITableSectionService
{
    private readonly ITableSectionRepository _tableSectionRepository;

    public TableSectionService(ITableSectionRepository tableSectionRepository)
    {
        _tableSectionRepository = tableSectionRepository;

    }
    public async Task<IEnumerable<SectionViewModel>> GetSectionsList()
    {
        IEnumerable<Section> sections = await _tableSectionRepository.GetSectionsListAsync();
        return sections.Select(s => new SectionViewModel{
            SectionID = s.SectionId,
            Name = s.Name,
            Description = s.Description
        });
    }

    public async Task<TableListViewModel> GetTableList(long sectionId,int pageNo, int pageSize, string search)
    {
        TableListViewModel model = new() {Page = new ()};
        var tableData = await _tableSectionRepository.GetTableListAsync(sectionId, pageNo, pageSize, search);

        model.TableList = tableData.tables;
        model.Page.SetPagination(tableData.totalRecords, pageSize, pageNo);
        return model;
    }
}
