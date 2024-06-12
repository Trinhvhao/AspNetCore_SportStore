using System.Diagnostics;
using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class HomeController : Controller
{
    private readonly SportStoreDb2Context _context; 

    public HomeController(SportStoreDb2Context context)
    {
        _context = context; 
    }

    public IActionResult Index()
    {
        // lấy userID từ session (giả sử đã được lưu trước đó)
        var userId = HttpContext.Session.GetInt32("userID");

        // gán giá trị userID vào ViewBag
        ViewBag.UserID = userId;
        // thực hiện truy vấn để lấy thông tin sản phẩm 
        var productsWithImage = _context.Products
            .Include(p => p.Images) 
            .ToList();

        return View(productsWithImage);
    }


    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Blog()
    {
        return View();
    }
}