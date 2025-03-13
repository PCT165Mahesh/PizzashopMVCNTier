using System.Threading.Tasks;
using BusinessLogicLayer.Implementations;
using DataLogicLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;

public class TableAndSectionController : Controller
{
    private readonly ITableSectionService _tableSectionService;


    public TableAndSectionController(ITableSectionService tableSectionService)
    {
        _tableSectionService = tableSectionService;

    }
    
    public async Task<IActionResult> Index()
    {
        IEnumerable<SectionViewModel> model =  await _tableSectionService.GetSectionsList();
        ViewData["ActiveLink"] = "Tables";
        return View(model);
    }

    public async Task<IActionResult> GetTableList(long sectionId = 1, int pageNo = 1, int pageSize = 3, string search = "")
    {
        return PartialView("_tablesList", await _tableSectionService.GetTableList(sectionId, pageNo, pageSize, search));
    }

}
