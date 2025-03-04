using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Interfaces;

public interface ICategoryItemService
{
    public List<CategoryViewModel> GetCategories();
    public CategoryViewModel GetCategoryById(long id);

    public Task<bool> AddCategory(CategoryViewModel model, string userName);
    public Task<bool> EditCategory(CategoryViewModel model, string userName);
    public Task<bool> DeleteCategory(long categoryId, string userName);

    public Task<ItemListViewModel> GetItemList(long categoryId,int pageNo, int pageSize, string search);
    
}
