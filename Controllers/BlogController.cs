using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Project.Controllers;

public class BlogController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}