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

    public RolePermissionController(IRoleService roleService, IUserDetailService userDetailService)
    {
        _roleService = roleService;
        _userDetailService = userDetailService;
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

    
    [HttpGet]
    public async Task<IActionResult> Permission(long id){
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;


        
        Role roleObj = await _roleService.GetRoleByIdAsync(id);
        string roleName = roleObj.Rolename;
        ViewData["RoleName"] = roleName;
        return View();
    }
}
