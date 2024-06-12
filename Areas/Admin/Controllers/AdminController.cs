using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminController : Controller
{
    private readonly SportStoreDb2Context _context;

    public AdminController(SportStoreDb2Context context)
    {
        _context = context;
    }

    [HttpGet("Admin/Index")]
    public async Task<IActionResult> Index()
    {
        
        var userId = HttpContext.Session.GetInt32("userID");
        // lấy thông tin người dùng từ database
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.Role != "Admin")
        {
            // Nếu userID không tồn tại hoặc không có quyền "Admin", chuyển hướng đến trang cần thiết
            return RedirectToAction("AccessDenied", "Admin");
        }
        
        // lấy ngày hiện tại
        var today = DateTime.Today;
        // lấy ra các đơn hàng được tạo vào ngày hiện tại
        var ordersToday = _context.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.OrderDate.Date == today)
            .ToList();
        // lấy danh sách người dùng mới đăng ký trong ngày hôm nay
        var newUsers = await _context.Users
            .Where(u => u.CreatedAt.Date == today)
            .ToListAsync();
        // lưu danh sách người dùng mới vào ViewBag
        ViewBag.NewUsers = newUsers;
        // lấy số lượng đơn hàng
        var orderCount = await _context.Orders.CountAsync();
        // lấy số lượng người dùng
        var userCount = await _context.Users.CountAsync();
        // lấy số lượng sản phẩm
        var productCount = await _context.Products.CountAsync();
        // lấy số lượng loại hàng
        var categoryCount = await _context.Categories.CountAsync();
        // gửi dữ liệu đến view
        ViewData["OrderCount"] = orderCount;
        ViewData["UserCount"] = userCount;
        ViewData["ProductCount"] = productCount;
        ViewData["CategoryCount"] = categoryCount;
        return View(ordersToday); // Load the view based on viewName
    }


    public IActionResult AccessDenied()
    {
        return View();
    }
}