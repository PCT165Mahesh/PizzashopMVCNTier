using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface ITableSectionRepository
{
    public Task<List<Section>> GetSectionsListAsync();
    public Task<(List<TableViewModel> tables, int totalRecords)> GetTableListAsync(long sectionId, int pageNo, int pageSize, string search);

}
