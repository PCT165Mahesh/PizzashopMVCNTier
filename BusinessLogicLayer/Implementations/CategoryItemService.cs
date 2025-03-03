using System.Net.Http.Headers;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace BusinessLogicLayer.Implementations;

public class CategoryItemService : ICategoryItemService
{
    private readonly ICategoryItemRepository _categoryItemRepository;
    private readonly IUserRepository _userRepository;

    public CategoryItemService(ICategoryItemRepository categoryItemRepository, IUserRepository userRepository)
    {
        _categoryItemRepository = categoryItemRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> AddCategory(CategoryViewModel model, string userName)
    {
        var user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        if(model == null)
        {
            return false;
        }

        return await _categoryItemRepository.AddCategoryAsync(model, user.Id);
    }

    

    public List<CategoryViewModel> GetCategories()
    {
        List<Category> categories = _categoryItemRepository.GetAllCategories();
        return categories.Select( c => new CategoryViewModel {Id = c.CategoryId ,CategoryName = c.Name, Description = c.Description}).ToList();
    }

    public CategoryViewModel GetCategoryById(long id)
    {
        if(id == null || id == -1)
        {
            return new CategoryViewModel();
        }

        return _categoryItemRepository.GetCategoryId(id);
    }

    public async Task<bool> EditCategory(CategoryViewModel model, string userName)
    {

        var user = await _userRepository.GetUserByUserName(userName);
        if(user == null)
        {
            return false;
        }

        if(model == null)
        {
            return false;
        }
        return await _categoryItemRepository.EditCategoryAsync(model, user.Id);
    }

}
