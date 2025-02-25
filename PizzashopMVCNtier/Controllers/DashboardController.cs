using System.Threading.Tasks;
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


    public DashboardController(JwtService jwtService, IUserDetailService userDetailService, ICountryService countryService, IChangePasswordService changePasswordService)
    {
        _jwtService = jwtService;
        _userDetailService = userDetailService;
        _countryService = countryService;
        _changePasswordService = changePasswordService;

    }


    /* ------------------------------------------------------------------------------------------------------Dashboard Action-------------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region 
    public IActionResult Index()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];


        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);


        ViewData["ActiveLink"] = "Dashboard";

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View();
    }
    #endregion



    /* ----------------------------------------------------------------------------------------------Profile Details Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/


    #region 
    [HttpGet]
    public async Task<IActionResult> ProfileDetails()
    {

        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _userDetailService.Email(token);
        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        ProfileDataViewModel model = await _userDetailService.GetProfileData(email);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ProfileDetails(ProfileDataViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _userDetailService.Email(token);

        if (ModelState.IsValid)
        {
            var result = await _userDetailService.UpdateUserProfileData(model, email);

            if (result)
            {
                return RedirectToAction("ProfileDetails");
            }
            return View(model);
        }
        return View(model);
    }

    #endregion


    /* ----------------------------------------------------------------------------------------------Change Password Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/


    #region 
    [HttpGet]
    public IActionResult ChangePassword()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

        var userName = _userDetailService.UserName(token);
        var imgUrl = _userDetailService.ImgUrl(token);

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePassViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

        var email = _userDetailService.Email(token);

        var result = await _changePasswordService.ChangePassword(model.CurrentPassword, model.NewPassword, model.ConfirmNewPassword, email);
        if (result)
        {
            TempData["SuccessMessage"] = "Password updated successfully!";
            return RedirectToAction("ChangePassword");
        }
        TempData["ErrorMessage"] = "Current password is incorrect.";
        return RedirectToAction("ChangePassword");
    }

    #endregion
    /* ----------------------------------------------------------------------------------------------Country State city Details Action----------
-------------------------------------------------------------------------------------------------------------------------------------*/

    #region 

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

    // [HttpGet]
    // public JsonResult GetRoles()
    // {

    //     var roles = _context.Roles.ToList();
    //     return Json(roles);
    // }

    #endregion
}
