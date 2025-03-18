using System.Threading.Tasks;
using BusinessLogicLayer.Common;
using BusinessLogicLayer.Implementations;
using BusinessLogicLayer.Interfaces;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;

public class TableAndSectionController : Controller
{
    private readonly ITableSectionService _tableSectionService;
    private readonly IUserDetailService _userDetailService;


    public TableAndSectionController(ITableSectionService tableSectionService, IUserDetailService userDetailService)
    {
        _tableSectionService = tableSectionService;
        _userDetailService = userDetailService;
    }

/*---------------------------------------------------------------------------Sectoin and Table List------------------------------------------------------------------------------*/

    #region Section and Table List   
    [PermissionAuthorize("TableAndSection_View")]
    public async Task<IActionResult> Index()
    {
        IEnumerable<SectionViewModel> model = await _tableSectionService.GetSectionsList();
        ViewData["ActiveLink"] = "Tables";
        return View(model);
    }

    public async Task<IActionResult> GetTableList(long sectionId = 1, int pageNo = 1, int pageSize = 3, string search = "")
    {
        return PartialView("_tablesList", await _tableSectionService.GetTableList(sectionId, pageNo, pageSize, search));
    }
    #endregion

/*---------------------------------------------------------------------------Sections CRUD------------------------------------------------------------------------------*/

    #region Add/Edit Section
    [HttpGet]
    [PermissionAuthorize("TableAndSection_AddEdit")]
    public async Task<IActionResult> SaveSection(long sectionId)
    {
        SectionViewModel model = new SectionViewModel();

        if (sectionId > 0)
        {
            model = await _tableSectionService.GetSectionById(sectionId);
            return PartialView("_addSection", model);
        }
        return PartialView("_addSection", model);
    }

    [HttpPost]
    [PermissionAuthorize("TableAndSection_AddEdit")]
    public async Task<IActionResult> SaveSection(SectionViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);
        long userId = await _userDetailService.GetUserIdByUserNameAsync(userName);


        if (!ModelState.IsValid)
        {
            return PartialView("_addSection", model);
        }


        string result = "";
        bool isCreated = true;

        //For Adding New Item
        if (model.SectionID == 0)
        {
            result = await _tableSectionService.AddSection(model, userId);
        }
        //For Editing Item
        else
        {
            result = await _tableSectionService.EditSection(model, userId);
            isCreated = false;
        }
        // Checking for Add or Update
        if (result.Equals("true"))
        {
            var message = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "Section");
            return Json(new { success = true, message = message });
        }
        else
        {
            return Json(new { success = false, errorMessage = result});
        }
    }
    #endregion

    #region Delete Section
    [HttpPost]
    [PermissionAuthorize("TableAndSection_Delete")]
    public async Task<IActionResult> DeleteSection(long sectionId)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _tableSectionService.DeleteSection(sectionId, userName);
        if (result)
        {
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "Section") });
        }
        else
        {
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "Section") });
        }
    }
    #endregion

/*---------------------------------------------------------------------------Tables CRUD------------------------------------------------------------------------------*/
    #region Add/Edit Table
    [HttpGet]
    [PermissionAuthorize("TableAndSection_AddEdit")]
    public async Task<IActionResult> SaveTable(long tableId)
    {
        TableViewModel model = new TableViewModel();
        model.SectionList = await _tableSectionService.GetSectionsList();


        if (tableId > 0)
        {
            model = await _tableSectionService.GetTableById(tableId);
            model.SectionList = await _tableSectionService.GetSectionsList();
            return PartialView("_addTable", model);
        }
        return PartialView("_addTable", model);
    }

    [HttpPost]
    [PermissionAuthorize("TableAndSection_AddEdit")]
    public async Task<IActionResult> SaveTable(TableViewModel model)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);
        long userId = await _userDetailService.GetUserIdByUserNameAsync(userName);


        if (!ModelState.IsValid)
        {
            model.SectionList = await _tableSectionService.GetSectionsList();
           return PartialView("_addTable", model);
        }


        string result = "";
        bool isCreated = true;

        //For Adding New Item
        if (model.TableId == 0 || model.TableId == null)
        {
            result = await _tableSectionService.AddTable(model, userId);
        }
        //For Editing Item
        else
        {
            result = await _tableSectionService.EditTable(model, userId);
            isCreated = false;
        }
        // Checking for Add or Update
        if (result.Equals("true"))
        {
            var message = string.Format(isCreated ? NotificationMessages.EntityCreated : NotificationMessages.EntityUpdated, "Table");
            return Json(new {success=true, message=message});
        }
        else
        {
            return Json(new {success=false, errorMessage= result});
        }
    }
    #endregion

    #region Delete Table
    [HttpPost]
    [PermissionAuthorize("TableAndSection_Delete")]
    public async Task<IActionResult> DeleteTable(long tableId, long sectionId)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _tableSectionService.DeleteTable(sectionId,tableId, userName);
        if (result)
        {
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "Table") });
        }
        else
        {
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "Table") });
        }
    }
    #endregion

    #region Mass Delte Table
    [HttpPost]
    [PermissionAuthorize("Menu_Delete")]
    public async Task<IActionResult> DeleteSelectedTables(List<long> id,long sectionId)
    {
        string? token = HttpContext.Session.GetString("SuperSecretAuthToken");
        string userName = _userDetailService.UserName(token);

        bool result = await _tableSectionService.DeleteSelectedTable(id,sectionId, userName);
        if (result)
        {
            return Json(new { success = true, message = string.Format(NotificationMessages.EntityDeleted, "Tables") });
        }
        else
        {
            return Json(new { success = false, message = string.Format(NotificationMessages.EntityDeletedFailed, "Table") });
        }
    }
    #endregion
}
