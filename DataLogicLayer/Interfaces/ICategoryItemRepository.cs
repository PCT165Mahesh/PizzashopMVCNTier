using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;

namespace DataLogicLayer.Interfaces;

public interface ICategoryItemRepository
{
    public List<Category> GetAllCategories();
    public CategoryViewModel GetCategoryId(long id);

    public Task<bool> AddCategoryAsync(CategoryViewModel model, long userId);
    public Task<bool> EditCategoryAsync(CategoryViewModel model, long userId);
    public Task<bool> DeleteCategoryAsync(long categoryId, long userId);

    public Task<(List<ItemsViewModel> items, int totalRecords)> GetItemList(long categoryId, int pageNo, int pageSize, string search);
}
