using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Implementations;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PizzashopMVCNtier.Controllers;

public class MenuController : Controller
{
    private readonly ICategoryItemService _categoryItemService;
    private readonly IUserDetailService _userDetailService;


    public MenuController(ICategoryItemService categoryItemService, IUserDetailService userDetailService)
    {
        _categoryItemService = categoryItemService;
        _userDetailService = userDetailService;
    }

    #region Menu Home Page
    [HttpGet]
    public IActionResult Index()
    {
        MenuViewModel model = new()
        {
            CategoryList = _categoryItemService.GetCategories(),
            ItemModel = new ItemListViewModel() {ItemList = {}, Page = new()},
            AddItems = new AdditemViewModel()
        };
        ViewData["ActiveLink"] = "Menu";
        return View(model);
    }
    #endregion

    #region Save Categories
    [HttpPost]
    public async Task<IActionResult> SaveCategory(MenuViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Menu");
        }
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = false;
        bool isCreated = true;

        //For Adding New Category
        if (model.Category.Id == -1)
        {
            result = await _categoryItemService.AddCategory(model.Category, userName);
        }
        //For Editing Category
        else
        {
            result = await _categoryItemService.EditCategory(model.Category, userName);
            isCreated = false;
        }
        // Checking for Add or Update
        if (result)
        {
            TempData["NotificationMessage"] = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "Category");
            TempData["NotificationType"] = NotificationType.Success.ToString();
        }
        else
        {
            TempData["NotificationMessage"] = string.Format(isCreated ? NotificationMessages.EntityCreatedFailed : NotificationMessages.EntityUpdatedFailed, "Category");
            TempData["NotificationType"] = NotificationType.Error.ToString();
        }
        return RedirectToAction("Index", "Menu");
    }

    #endregion

    #region Delete Category
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(long id){
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _categoryItemService.DeleteCategory(id, userName);
        if(result){
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "Category") });
        }
        else{
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "Category") });
        }
    }
    #endregion

    #region Crud Item

    [HttpGet]
    public IActionResult SaveItem(long? id)
    {
        AdditemViewModel model = new AdditemViewModel();
        model.CategoryList = _categoryItemService.GetCategories();
        model.ItemTypeList =  _categoryItemService.GetItemtypes();
        model.UnitList = _categoryItemService.GetUnits();

        return PartialView("_addItemModalPartialView", model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveItem(AdditemViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);
        long userId = await _userDetailService.GetUserIdByUserNameAsync(userName);

        if(!ModelState.IsValid)
        {
            TempData["NotificationMessage"] = NotificationMessages.InvalidModelState;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return PartialView("_addItemModalPartialView", model);
        }

        string result = "";
        bool isCreated = true;

        //For Adding New Item
        if (model.ItemId == -1)
        {
            result = await _categoryItemService.AddItem(model, userId);
        }
        //For Editing Item
        else
        {
            result = await _categoryItemService.EditItem(model, userId);
            isCreated = false;
        }
        // Checking for Add or Update
        if (result.Equals("true"))
        {
            TempData["NotificationMessage"] = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "Item");
            TempData["NotificationType"] = NotificationType.Success.ToString();
        }
        else
        {
            TempData["NotificationMessage"] = result;
            TempData["NotificationType"] = NotificationType.Error.ToString();
        }
        return RedirectToAction("Index");
    }
    #endregion

    #region Items By Category
    public async Task<IActionResult> GetItemList(long categoryId = 1, int pageNo = 1, int pageSize = 3, string search = "")
    {
        return PartialView("_itemsPartialView", await _categoryItemService.GetItemList(categoryId, pageNo, pageSize, search));
    }
    #endregion

    #region All Categories In JSON
    public IActionResult GetCategoryById(long id)
    {
        return Json(_categoryItemService.GetCategoryById(id));
    }
    #endregion
}
