using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class UserController : Controller
{
    private readonly SportStoreDb2Context _context;

    public UserController(SportStoreDb2Context context)
    {
        _context = context;
    }

    // action để lấy thông tin người dùng
    public async Task<IActionResult> Index()
    {
        // lấy userID từ session
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null)
        {
            // chuyển hướng đến trang đăng nhập nếu không có userID
            return RedirectToAction("Index", "Register");
        }


        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            // người dùng chưa đăng nhập
            HttpContext.Session.SetString("ReturnUrl", Url.Action("Index", "User"));
            return NotFound();
        }

        // lấy danh sách đơn hàng của người dùng cùng với thông tin sản phẩm trong mỗi đơn hàng
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product) // Include Product in OrderDetails
            .ToListAsync();
        
        var viewModel = new UserOrderViewModel
        {
            User = user,
            Orders = orders
        };
        
        return View(viewModel);
    }
    // action để chỉnh sửa thông tin người dùng
    [HttpPost]
    public async Task<IActionResult> Edit([Bind("FullName, Address, Email, PhoneNumber")] User user)
    {
        // lấy userID từ session
        var userId = HttpContext.Session.GetInt32("userID");
        var existingUser = await _context.Users.FindAsync(userId);
        if (existingUser == null)
        {
            // xử lý khi người dùng không tồn tại
            return NotFound();
        }
        // cập nhật các trường dữ liệu được chỉ định
        existingUser.FullName = user.FullName;
        existingUser.Address = user.Address;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;

        // cập nhật thay đổi 
        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        
        TempData["SuccessMessage"] = 1;
     
        return RedirectToAction("Index"); 
    }
    // đăng xuất người dùng
    public IActionResult Logout()
    {
        // xóa thông tin phiên liên quan đến người dùng
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Register");
    }
    

}

