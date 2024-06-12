using System.Diagnostics;
using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class HomeController : Controller
{
    private readonly SportStoreDb2Context _context; // Đối tượng DbContext

    public HomeController(SportStoreDb2Context context)
    {
        _context = context; // Khởi tạo đối tượng DbContext
    }

    public IActionResult Index()
    {
        // Lấy userID từ session (giả sử đã được lưu trước đó)
        var userId = HttpContext.Session.GetInt32("userID");

        // Gán giá trị userID vào ViewBag
        ViewBag.UserID = userId;
        // Thực hiện truy vấn để lấy thông tin sản phẩm và đường dẫn hình ảnh từ bảng Image
        var productsWithImage = _context.Products
            .Include(p => p.Images) // Đảm bảo rằng đối tượng Image cũng được load
            .ToList();

        return View(productsWithImage);
    }


    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        // Logic for the services page
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Blog()
    {
        return View();
    }
}