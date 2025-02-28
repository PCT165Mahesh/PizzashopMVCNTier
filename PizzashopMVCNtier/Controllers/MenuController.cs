using Microsoft.AspNetCore.Mvc;

namespace PizzashopMVCNtier.Controllers;

public class MenuController : Controller
{
    [HttpGet]
    public IActionResult Index(){
        ViewData["ActiveLink"] = "Menu";
        return View();
    }
}
