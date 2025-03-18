using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Constants;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;


public class UserController : Controller
{
    private readonly IUserDetailService _userDetailService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ICountryService _countryService;

    public UserController(IUserDetailService userDetailService, IUserService userService, IRoleService roleService, ICountryService countryService)
    {
        _userDetailService = userDetailService;
        _userService = userService;
        _roleService = roleService;
        _countryService = countryService;
    }


    #region User List
    [PermissionAuthorize("Users_View")]
    public IActionResult Index()
    {
        ViewData["ActiveLink"] = "User";
        return View(new UserViewModel() { UserList = {}, Page = new() });
    }



    [HttpGet]   
    public async Task<IActionResult> GetUserList(int pageNo = 1, int pageSize = 3, string search = "",string columnName="", string sortOrder="")
    {
        return PartialView("_userListPartialView",await _userDetailService.GetUserDetails(pageNo, pageSize, search, columnName, sortOrder));
    }

    #endregion

    #region Add/Edit User

    [HttpGet]
    [PermissionAuthorize("Users_AddEdit")]
    public async Task<IActionResult> SaveUser(long? id)
    {
        AddUserViewModel model = new AddUserViewModel();
        if (id.HasValue && id > 0)
        {
            model = await _userService.GetUserByIdAsync(id.Value);
        }
        await PopulateDropdownLists(model);
        return View(model);
    }

    [HttpPost]
    [PermissionAuthorize("Users_AddEdit")]
    public async Task<IActionResult> SaveUser(AddUserViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        if (!ModelState.IsValid)
        {
            model.UserName = await _userDetailService.UsernameByEmail(model.Email);
            await PopulateDropdownLists(model);
            return View(model);
        }
        var message = "";
        bool result = false;
        bool isCreated = true;

        if(model.UserId.HasValue && model.UserId.Value > 0)
        {
            (message, result) = await _userService.UpdateUserAsync(model, userName);
            isCreated = false;
        }
        else
        {
            (message, result) = await _userService.AddUserAsync(model, userName);
        }
        if (result)
        {
            TempData["NotificationMessage"] = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "User");
            TempData["NotificationType"] = NotificationType.Success.ToString();
        }
        else
        {
            TempData["NotificationMessage"] = message;
            TempData["NotificationType"] = NotificationType.Error.ToString();
            // model.UserName = await _userDetailService.UsernameByEmail(model.Email);
            await PopulateDropdownLists(model);
            return View(model);
        }
        return RedirectToAction("Index", "User");
    }
    #endregion

    #region Delete User
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("Users_Delete")]

    public async Task<IActionResult> DeleteUser(long id)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _userService.DeleteUserAsync(id, userName);
        if (result)
        {
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "User") });
        }
        else
        {
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "User") });
        }
    }
    #endregion

    #region Populate List
    private async Task PopulateDropdownLists(AddUserViewModel model)
    {
        // model.UserName = await _userDetailService.UsernameByEmail(model.Email);
        model.Roles = await _roleService.GetRolesAsync();
        model.Countries = _countryService.GetCountries();
        model.States = model.CountryId > 0 ? _countryService.GetStates(model.CountryId) : new List<State>();
        model.Cities = model.StateId > 0 ?  _countryService.GetCities(model.StateId) : new List<City>();
    }
    #endregion
}
