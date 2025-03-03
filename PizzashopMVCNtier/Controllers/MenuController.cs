using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Implementations;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet]
    public IActionResult Index()
    {
        MenuViewModel model = new()
        {
            CategoryList = _categoryItemService.GetCategories(),
        };
        ViewData["ActiveLink"] = "Menu";
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Categories(MenuViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
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



    public IActionResult GetCategoryById(long id)
    {
        return Json(_categoryItemService.GetCategoryById(id));
    }
}
