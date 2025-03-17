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
    /*---------------------------------------------------------------------------Sections CRUD------------------------------------------------------------------------------*/

    #region Get Sections List
    public async Task<Section> GetSectionByIdAsync(long sectionId)
    {
        return await _context.Sections.Where(s => s.SectionId == sectionId && !s.Isdeleted).FirstOrDefaultAsync();
    }
    public Task<List<Section>> GetSectionsListAsync()
    {
        return _context.Sections.Where(s => !s.Isdeleted).ToListAsync();
    }
    #endregion

    #region ADD : Section
    public async Task<string> AddSectionAsync(SectionViewModel model, long userId)
    {
        Section? existingSection = await _context.Sections.Where(s => s.Name == model.Name && s.SectionId != model.SectionID && !s.Isdeleted).FirstOrDefaultAsync();
        if (existingSection != null && existingSection.Isdeleted == false)
        {
            return $"{model.Name} Section already exist! ";
        }
        if (existingSection != null && existingSection.Isdeleted == true)
        {
            existingSection.Name = string.Concat(existingSection.Name, DateTime.Now);
            _context.Sections.Update(existingSection);
            await _context.SaveChangesAsync();
        }

        try
        {
            Section newSection = new Section
            {
                Name = model.Name,
                Description = model.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await _context.Sections.AddAsync(newSection);
            await _context.SaveChangesAsync();
            return "true";

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error In Table and Section Repository :", ex.Message);
            return "Failed To Add Section";
        }
    }
    #endregion

    #region EDIT : Section
    public async Task<string> EditSectionAsync(SectionViewModel model, long userId)
    {
        try
        {
            Section? existingSection = await _context.Sections.Where(s => s.Name == model.Name && s.SectionId != model.SectionID && !s.Isdeleted).FirstOrDefaultAsync();
            if (existingSection != null && existingSection.Isdeleted == false)
            {
                return $"{model.Name} Section already exist! ";
            }
            if (existingSection != null && existingSection.Isdeleted == true)
            {
                existingSection.Name = string.Concat(existingSection.Name, DateTime.Now);
                _context.Sections.Update(existingSection);
                await _context.SaveChangesAsync();
            }

            Section? section = await _context.Sections.Where(s => s.SectionId == model.SectionID && !s.Isdeleted).FirstOrDefaultAsync();

            if(section == null)
            {
                return "Section Does not exists";
            }

            section.Name = model.Name;
            section.Description = model.Description;
            section.UpdatedBy = userId;
            section.UpdatedAt = DateTime.Now;
            _context.Sections.Update(section);
            await _context.SaveChangesAsync();
            return "true"; 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Updating Section", ex.Message);
            return "Failed To Updated Section";
        }
    }
    #endregion

    #region DELETE : Section
    public async Task<bool> DeleteSectionAsync(long sectionId, long userId)
    {
        try
        {
            Section? section = await _context.Sections.Where(s => s.SectionId == sectionId && !s.Isdeleted).FirstOrDefaultAsync();

            if(section == null) return false;

            List<Table> tables = await _context.Tables.Where(t => t.Sectionid == sectionId && !t.Isdeleted).ToListAsync();
            foreach(Table table in tables)
            {
                table.Isdeleted = true;
                table.UpdatedBy = userId;
                table.UpdatedAt = DateTime.Now;
                _context.Tables.Update(table);
                await _context.SaveChangesAsync();
            }

            section.Isdeleted = true;
            section.UpdatedAt = DateTime.Now;
            section.UpdatedBy = userId;
            _context.Sections.Update(section);
            await _context.SaveChangesAsync();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error In Delete item Repository: ", ex.Message);
            return false;
        }

    }
    #endregion

    #region Tables List
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

    public async Task<Table> GetTableByIdAsync(long tableId)
    {
        return await _context.Tables.Where(t => t.Id == tableId && !t.Isdeleted).FirstOrDefaultAsync();
    }
    #endregion

    #region ADD : Table
    public async Task<string> AddTableAsync(TableViewModel model, long userId)
    {
        Table? existingTable = await _context.Tables.Where(t => t.Name == model.TableName && t.Id != model.TableId && t.Sectionid == model.SectionId && !t.Isdeleted).FirstOrDefaultAsync();
        if (existingTable != null && existingTable.Isdeleted == false)
        {
            return $"{model.TableName} Table already exist! ";
        }
        if (existingTable != null && existingTable.Isdeleted == true)
        {
            existingTable.Name = string.Concat(existingTable.Name, DateTime.Now);
            _context.Tables.Update(existingTable);
            await _context.SaveChangesAsync();
        }

        try
        {
            Table newTable = new Table
            {
                Name = model.TableName,
                Sectionid = model.SectionId,
                Capacity = model.Capacity,
                IsOccupied = model.IsOccupied,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await _context.Tables.AddAsync(newTable);
            await _context.SaveChangesAsync();
            return "true";

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error In Table and Section Repository :", ex.Message);
            return "Failed To Add Table";
        }
    }
    #endregion

    #region EDIT : Table
    public async Task<string> EditTableAsync(TableViewModel model, long userId)
    {
        try
        {
            Table? existingTable = await _context.Tables.Where(t => t.Name == model.TableName && t.Id != model.TableId && t.Sectionid == model.SectionId && !t.Isdeleted).FirstOrDefaultAsync();
            if (existingTable != null && existingTable.Isdeleted == false)
            {
                return $"{model.TableName} Table already exist! ";
            }
            if (existingTable != null && existingTable.Isdeleted == true)
            {
                existingTable.Name = string.Concat(existingTable.Name, DateTime.Now);
                _context.Tables.Update(existingTable);
                await _context.SaveChangesAsync();
            }

            Table? table = await _context.Tables.Where(t => t.Id == model.TableId && !t.Isdeleted).FirstOrDefaultAsync();

            if(table == null)
            {
                return "Table Does not exists";
            }

            table.Name = model.TableName;
            table.Capacity = model.Capacity;
            table.UpdatedBy = userId;
            table.UpdatedAt = DateTime.Now;
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
            return "true"; 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Updating Table", ex.Message);
            return "Failed To Updated Table";
        }
    }
    #endregion

    #region DELETE : Table
    public async Task<bool> DeleteTableAsync(long sectionId, long tableId, long userId)
    {
        try
        {
            Table? table = await _context.Tables.Where(t => t.Id == tableId && t.Sectionid == sectionId).FirstOrDefaultAsync();

            if(table == null) return false;

            table.Isdeleted = true;
            table.UpdatedAt = DateTime.Now;
            table.UpdatedBy = userId;
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error In Delete table Repository: ", ex.Message);
            return false;
        }

    }
    #endregion
}
