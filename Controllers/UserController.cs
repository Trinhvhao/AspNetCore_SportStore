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

    // Action để lấy thông tin người dùng
    public async Task<IActionResult> Index()
    {
        // Lấy userID từ session
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null)
        {
            // Chuyển hướng đến trang đăng nhập nếu không có userID
            return RedirectToAction("Index", "Register");
        }

        // Lấy thông tin người dùng từ database
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            // Người dùng chưa đăng nhập, lưu URL hiện tại và chuyển hướng đến trang đăng nhập
            HttpContext.Session.SetString("ReturnUrl", Url.Action("Index", "User"));
            // Xử lý khi người dùng không tồn tại (nếu cần)
            return NotFound();
        }

        // Lấy danh sách đơn hàng của người dùng cùng với thông tin sản phẩm trong mỗi đơn hàng
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product) // Include Product in OrderDetails
            .ToListAsync();

        // Tạo ViewModel và gán dữ liệu
        var viewModel = new UserOrderViewModel
        {
            User = user,
            Orders = orders
        };

        // Trả về view với ViewModel
        return View(viewModel);
    }
    // Action để chỉnh sửa thông tin người dùng
    [HttpPost]
    public async Task<IActionResult> Edit([Bind("FullName, Address, Email, PhoneNumber")] User user)
    {
        // Lấy userID từ session
        var userId = HttpContext.Session.GetInt32("userID");
        var existingUser = await _context.Users.FindAsync(userId);
        if (existingUser == null)
        {
            // Xử lý khi người dùng không tồn tại
            return NotFound();
        }
        // Cập nhật các trường dữ liệu được chỉ định
        existingUser.FullName = user.FullName;
        existingUser.Address = user.Address;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;

        // Cập nhật thay đổi vào cơ sở dữ liệu
        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        // Đặt thông báo vào TempData
        TempData["SuccessMessage"] = 1;
        // Chuyển hướng người dùng đến một trang khác sau khi chỉnh sửa thành công
        return RedirectToAction("Index"); // Ví dụ: chuyển hướng đến trang chính của ứng dụng
    }
    // Action để đăng xuất người dùng
    public IActionResult Logout()
    {
        // Xóa thông tin phiên liên quan đến người dùng
        HttpContext.Session.Clear();

        // Chuyển hướng đến trang đăng nhập hoặc trang chính
        return RedirectToAction("Index", "Register");
    }
    

}

