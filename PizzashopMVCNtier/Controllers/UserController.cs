using BusinessLogicLayer.Common;
using BusinessLogicLayer.Constants;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;


public class UserController : Controller
{
    private readonly IUserDetailService _userDetailService;
    private readonly IUserService _userService;

    public UserController(IUserDetailService userDetailService, IUserService userService)
    {
        _userDetailService = userDetailService;
        _userService = userService;
    }

    /* ------------------------------------------------------------------------------------------------------User List Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/
    
    #region User List
    [Authorize]
    public IActionResult Index()
    {
        ViewData["ActiveLink"] = "User";
        return View();
    }



    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserList(int pageNo = 1, int pageSize = 3, string search = ""){
        return await _userDetailService.GetUserDetails(pageNo, pageSize, search);
    }

    #endregion
    /* ------------------------------------------------------------------------------------------------------Add User Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region Add User
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpGet]
    public async Task<IActionResult> AddUser()
    {
        ViewData["ActiveLink"] = "User";
        return View();
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if(!ModelState.IsValid){
            TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityCreatedFailed, "User");
            TempData["NotificationType"] = NotificationType.Error.ToString(); 
            return View(model);
        }
        
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        var userName = _userDetailService.UserName(token);
 
        bool result = await _userService.AddUserAsync(model, userName);
        if(result){
            TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityCreated, "User");
            TempData["NotificationType"] = NotificationType.Success.ToString(); 
            return RedirectToAction("Index");
        }

        TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityCreatedFailed, "User");
        TempData["NotificationType"] = NotificationType.Error.ToString(); 
        return RedirectToAction("AddUser", "User");
    }
    #endregion

    /* ------------------------------------------------------------------------------------------------------Edit User Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/
    #region Edit User
    [HttpGet]
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    public async Task<IActionResult> EditUser(int id)
    {
        var model = await _userService.GetUserByIdAsync(id);
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        var userName = _userDetailService.UserName(token);

        if(ModelState.IsValid){
            var result = await _userService.UpdateUserAsync(model, userName);
            if(result){
                TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityUpdated, "User");;
                TempData["NotificationType"] = NotificationType.Success.ToString(); 
                return RedirectToAction("Index", "User");
            }
            else{
                TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityUpdatedFailed, "User");;
                TempData["NotificationType"] = NotificationType.Error.ToString(); 
                return RedirectToAction("Index", "User");
            }
        }
        return View(model);
    }
    #endregion

    /* ------------------------------------------------------------------------------------------------------Delete User Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/
    #region Delete User
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]

    public async Task<IActionResult> DeleteUser(long id)
    {

        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        var userName = _userDetailService.UserName(token);

        var result = await _userService.DeleteUserAsync(id, userName);
        if(result){
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "User") });
        }
        else{
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "User") });
        }
    }
    #endregion
}
