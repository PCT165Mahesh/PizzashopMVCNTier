using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLogicLayer.Implementations;

public class TableSectionRepository : ITableSectionRepository
{
    private readonly PizzaShopDbContext _context;


    public TableSectionRepository(PizzaShopDbContext context)
    {
        _context = context;

    }
    public Task<List<Section>> GetSectionsListAsync()
    {
        return _context.Sections.Where(s => !s.Isdeleted).ToListAsync();
    }

    public async Task<(List<TableViewModel> tables, int totalRecords)> GetTableListAsync(long sectionId, int pageNo, int pageSize, string search)
    {
        IQueryable<TableViewModel> query = _context.Tables
        
                            .Where(t => t.Sectionid == sectionId && !t.Isdeleted)
                            .Select(t => new TableViewModel
                            {
                               TableId = t.Id,
                               SectionId = t.Sectionid,
                               TableName = t.Name,
                               Capacity = t.Capacity,
                               IsOccupied = t.IsOccupied,
                            });


        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(i => i.TableName.ToLower().Contains(search) ||
                                i.Capacity.ToString().Contains(search));
        }

        int totalRecords = await query.CountAsync();

        List<TableViewModel> tables = await query
                     .Skip((pageNo - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();

        return (tables, totalRecords);
        
    }

}
