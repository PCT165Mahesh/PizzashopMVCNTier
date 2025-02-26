using System.Diagnostics;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzashopMVCNtier.Models;

namespace PizzashopMVCNtier.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILoginService _loginService;
    private readonly IForgotPasswordService _forgotPasswordService;
    private readonly IResetPasswordService _resetPasswordService;

    public HomeController(ILogger<HomeController> logger, ILoginService loginService, IForgotPasswordService forgotPasswordService, IResetPasswordService resetPasswordService)
    {
        _logger = logger;
        _loginService = loginService;
        _forgotPasswordService = forgotPasswordService;
        _resetPasswordService = resetPasswordService;
    }


    /* ------------------------------------------------------------------------------------------------------Login Action-------------
    -------------------------------------------------------------------------------------------------------------------------------------*/

    #region Login
    [HttpGet]
    public IActionResult Login()
    {

        if (Request.Cookies["UserEmail"] != null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid Credentails");
            return View(model);
        }


        var token = await _loginService.LoginUser(model.Email, model.Password);
        if (token != null)
        {
            // Cookie 
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("SuperSecretAuthToken", token, options);

            if (model.RememberMe)
            {
                Response.Cookies.Append("UserEmail", model.Email, new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(5),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            TempData["SuccessMessage"] = "Login Successful!";
            return RedirectToAction("Index", "Dashboard");
        }

        TempData["ErrorMessage"] = "Invalid Email or Password!";
        return View(model);
        
    }
    #endregion


    /* -------------------------------------------------------------------------------------------------------Logout Action---------------
    --------------------------------------------------------------------------------------------------------------------------------------*/

    #region Logout
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserEmail");
        Response.Cookies.Delete("SuperSecretAuthToken");
        return RedirectToAction("Login", "Home");
    }
    #endregion

    /* ------------------------------------------------------------------------------------------------------Forgot Password Action-------------
    ---------------------------------------------------------------------------------------------------------------------------------------------*/

    #region Forgot Password
    [HttpGet]
    public IActionResult ForgotPassword(string email)
    {

        if (string.IsNullOrEmpty(email))
        {
            ViewBag.UserEmail = "";
        }
        ViewBag.UserEmail = email;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPassViewModel model)
    {
        if (ModelState.IsValid)
        {
            var emailToken = Guid.NewGuid().ToString();
            var resetPasswordLink = Url.Action("ResetPassword", "Home", new { model.Email, emailToken }, Request.Scheme);

            var result = await _forgotPasswordService.ForgotPassword(model.Email, resetPasswordLink);

            // If Email is send then Toast Message Display
            if (result)
            {
                TempData["SuccessMessage"] = "Email sent successfully";
            }
            else
            {
                ModelState.AddModelError("", "Email Is not found");
                TempData["ErrorMessage"] = "Email is not Register";
            }
            return View();
        }
        return View(model);
    }

    #endregion


    /* ------------------------------------------------------------------------------------------------------Reset Password Action-------------
    ---------------------------------------------------------------------------------------------------------------------------------------------*/

    #region Reset Password

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        ViewData["Email"] = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPassViewModel model, string email)
    {
        if (ModelState.IsValid)
        {
            var result = await _resetPasswordService.ResetPassword(model.Password, model.ConfirmPassword, email);

            // If password Reset Successfully
            if (result)
            {
                TempData["SuccessMessage"] = "Password Reset successfully";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                return View(model);
            }
        }
        return View(model);
    }

    #endregion


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
