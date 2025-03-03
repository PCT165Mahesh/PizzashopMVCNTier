using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Iana;

namespace PizzashopMVCNtier.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly JwtService _jwtService;
    private readonly IUserDetailService _userDetailService;
    private readonly ICountryService _countryService;
    private readonly IChangePasswordService _changePasswordService;
    private readonly IRoleService _roleService;

    
    public DashboardController(JwtService jwtService, IUserDetailService userDetailService, 
        ICountryService countryService, IChangePasswordService changePasswordService, IRoleService roleService)
    {
        _jwtService = jwtService;
        _userDetailService = userDetailService;
        _countryService = countryService;
        _changePasswordService = changePasswordService;
        _roleService = roleService;
    }


    /* ------------------------------------------------------------------------------------------------------Dashboard Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region Dashboard
    public IActionResult Index()
    {
        ViewData["ActiveLink"] = "Dashboard";
        return View();
    }
    #endregion


    /* ----------------------------------------------------------------------------------------------Profile Details Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region Profile Data
    [HttpGet]
    public async Task<IActionResult> ProfileDetails()
    {

        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        var email = _userDetailService.Email(token);

        ProfileDataViewModel model = await _userDetailService.GetProfileData(email);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ProfileDetails(ProfileDataViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        var email = _userDetailService.Email(token);

        if(!ModelState.IsValid){
            TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityUpdatedFailed, "Profile");
            TempData["NotificationType"] = NotificationType.Error.ToString();
            return View(model);
        }
        bool result = await _userDetailService.UpdateUserProfileData(model, email);

        if (result)
        {
            TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityUpdated, "Profile");
            TempData["NotificationType"] = NotificationType.Success.ToString();
            return RedirectToAction("ProfileDetails");
        }
        TempData["NotificationMessage"] = string.Format(NotificationMessages.EntityUpdatedFailed, "Profile");
        TempData["NotificationType"] = NotificationType.Error.ToString();
        return View(model);
    }

    #endregion


    /* ----------------------------------------------------------------------------------------------Change Password Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region Change Password
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePassViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");

        var email = _userDetailService.Email(token);

        var result = await _changePasswordService.ChangePassword(model.CurrentPassword, model.NewPassword, model.ConfirmNewPassword, email);
        if (result)
        {
            TempData["NotificationMessage"] = NotificationMessages.PasswordChanged;
            TempData["NotificationType"] = NotificationType.Success.ToString(); 
            return RedirectToAction("ChangePassword");
        }
        TempData["NotificationMessage"] = NotificationMessages.PasswordChangeFailed;
        TempData["NotificationType"] = NotificationType.Error.ToString(); 
        return RedirectToAction("ChangePassword");
    }

    #endregion
    /* ----------------------------------------------------------------------------------------------Country State city Details Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region Coutry, City, States, Roles

    [HttpGet]
    public JsonResult GetCountries()
    {
        var countries = _countryService.GetCountries();
        return Json(countries);
    }


    [HttpGet]
    public JsonResult GetStatesByCountry(long countryId)
    {
        var states = _countryService.GetStates(countryId);

        return Json(states);
    }


    [HttpGet]
    public JsonResult GetCitiesByState(long id)
    {
        Console.WriteLine($"State ID received: {id}");

        var cities = _countryService.GetCities(id);

        return Json(cities);
    }

    [HttpGet]
    public async Task<JsonResult> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Json(roles);
    }
    #endregion
}
