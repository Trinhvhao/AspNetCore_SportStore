using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Project.Controllers;

public class WishListController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}