using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;

public class TaxesAndFeesController : Controller
{
    private readonly ITaxesAndFeesService _taxesAndFeesService;
    private readonly IUserDetailService _userDetailService;

    public TaxesAndFeesController(ITaxesAndFeesService taxesAndFeesService, IUserDetailService userDetailService)
    {
        _taxesAndFeesService = taxesAndFeesService;
        _userDetailService = userDetailService;
    }
    #region Taxes CRUD
    public IActionResult Index()
    {
         ViewData["ActiveLink"] = "Texes";
        return View(new TaxViewModel() { TaxList = {}, Page = new() });
    }

    [HttpGet]   
    public async Task<IActionResult> GetTaxList(int pageNo = 1, int pageSize = 3, string search = "")
    {
        return PartialView("_taxList",await _taxesAndFeesService.GetTaxDetails(pageNo, pageSize, search));
    }

    [HttpGet]
    public async Task<IActionResult> AddEditTax(long taxId)
    {
        TaxListViewModel model = new TaxListViewModel();
        if(taxId > 0)
        {
            model = await _taxesAndFeesService.GetTaxById(taxId);
        }
        return PartialView("_addEditTax", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddEditTax(TaxListViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);
        long userId = await _userDetailService.GetUserIdByUserNameAsync(userName);


        if (!ModelState.IsValid)
        {
           return PartialView("_addEditTax", model);
        }


        string result = "";
        bool isCreated = true;

        //For Adding New Item
        if (model.TaxId == 0 || model.TaxId == null)
        {
            result = await _taxesAndFeesService.AddTax(model, userId);
        }
        //For Editing Item
        else
        {
            result = await _taxesAndFeesService.EditTax(model, userId);
            isCreated = false;
        }
        // Checking for Add or Update
        if (result.Equals("true"))
        {
            var message = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "Tax");
            return Json(new {success=true, message=message});
        }
        else
        {
            return Json(new {success=false, errorMessage= result});
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTax(long taxId)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _taxesAndFeesService.DeleteTax(taxId, userName);
        if (result)
        {
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "Tax") });
        }
        else
        {
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "Tax") });
        }
    }
    #endregion
}
