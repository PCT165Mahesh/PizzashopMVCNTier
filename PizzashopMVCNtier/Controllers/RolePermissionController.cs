using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Models;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;


[Authorize]
public class RolePermissionController :Controller
{
    private readonly IRoleService _roleService;
    private readonly IUserDetailService _userDetailService;
    private readonly IRolePermissionService _rolePermissionService;


    public RolePermissionController(IRoleService roleService, IUserDetailService userDetailService, IRolePermissionService rolePermissionService)
    {
        _roleService = roleService;
        _userDetailService = userDetailService;
        _rolePermissionService = rolePermissionService;

    }

    #region Role List
    [HttpGet]
    public async Task<IActionResult> Role(){
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;

        ViewData["Activelink"] = "Role";
        List<Role> roleObj = await _roleService.GetRolesAsync();

        return View(roleObj);
    }

    #endregion

    
    #region  Permissions Get and Post
    [HttpGet]
    public async Task<IActionResult> Permission(long id){
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;

        RolePermissionViewModel model = _rolePermissionService.GetRolePermissions(id);
        
        Role roleObj = await _roleService.GetRoleByIdAsync(id);
        string roleName = roleObj.Rolename;
        ViewData["RoleName"] = roleName;
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Permission(long roleId, List<PermissionsViewModel> model){
        var token = Request.Cookies["SuperSecretAuthToken"];
        var userName = _userDetailService.UserName(token);

        var userId = await _userDetailService.GetUserIdByUserNameAsync(userName);

        if(model == null){
            ModelState.AddModelError("", "Permission List is Empty");
            return View(model);
        }
        var result = await _rolePermissionService.EditRolepermissions(roleId ,model, userId);

        if(result){
            return RedirectToAction("Permission", "RolePermission");
        }

        return View(model);
    }
    #endregion
}
