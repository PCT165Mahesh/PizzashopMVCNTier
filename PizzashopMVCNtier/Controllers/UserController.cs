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
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);


        ViewData["ActiveLink"] = "User";

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
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
    [Authorize(Roles = "Super Admin")]
    [HttpGet]
    public async Task<IActionResult> AddUser()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        var userId =await _userDetailService.GetUserIdByUserNameAsync(userName);

        ViewData["ActiveLink"] = "User";

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        ViewData["userId"] = userId;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Super Admin")]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];
        var userName = _userDetailService.UserName(token);

        if (ModelState.IsValid)
        {
            var result = await _userService.AddUserAsync(model);
            if(result){
                TempData["SuccessMessage"] = "User Added successfully! Email Sent";
                return RedirectToAction("AddUser", "User");
            }
            TempData["ErrorMessage"] = "Error Adding User!";
            return RedirectToAction("Index");
        }
        // TempData["ErrorMessage"] = "Error Adding User!";
        return View(model);
    }
    #endregion

    /* ------------------------------------------------------------------------------------------------------Edit User Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/
    #region Edit User
    [Authorize(Roles = "Super Admin")]
    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        var model = await _userService.GetUserByIdAsync(id);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View(model);
    }
    [Authorize(Roles = "Super Admin")]
    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];
        var userName = _userDetailService.UserName(token);

        if(ModelState.IsValid){
            var result = await _userService.UpdateUserAsync(model, userName);
            if(result){
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("Index", "User");
            }
            else{
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Index", "User");
            }
        }
        return View(model);
    }
    #endregion

    /* ------------------------------------------------------------------------------------------------------Delete User Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/
    #region Delete User
    [Authorize(Roles = "Super Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(long id)
    {

        var token = Request.Cookies["SuperSecretAuthToken"];
        var userName = _userDetailService.UserName(token);

        var result = await _userService.DeleteUserAsync(id, userName);
        if(result){
            return Json(new { success = true, message = "User deleted successfully!" });
        }
        else{
            return Json(new { success = false, message = "User not found!" });
        }
    }
    #endregion
}
