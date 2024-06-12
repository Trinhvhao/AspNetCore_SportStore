using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd_Project.Controllers;

public class RegisterController : Controller
{
    private readonly SportStoreDb2Context _context;

    public RegisterController(SportStoreDb2Context context)
    {
        _context = context;
    }

    // GET: /Register
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Register
    [HttpPost]
    public IActionResult Index(User model)
    {
        if (!ModelState.IsValid) return View(model);
        // Thêm logic để lưu dữ liệu đăng ký vào database
        var user = new User
        {
            Username = model.Username,
            Password = model.Password,
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role, // Đặt vai trò mặc định là "Customer"
            Address = model.Address,
            CreatedAt = DateTime.Now
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        // Lưu thông tin userID vào session
        HttpContext.Session.SetInt32("userID", user.UserId);

        // Kiểm tra và lấy URL từ session
        var returnUrl = HttpContext.Session.GetString("ReturnUrl");

        if (!string.IsNullOrEmpty(returnUrl))
        {
            // Xóa ReturnUrl khỏi session
            HttpContext.Session.Remove("ReturnUrl");
            return Redirect(returnUrl);
        }

        // Chuyển hướng người dùng đến trang chủ của website
        return RedirectToAction("Index", "Home"); // Chuyển hướng sau khi đăng ký thành công
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.Username == username && u.Password == password);

        if (user != null)
        {
            // Lưu thông tin userID vào session
            HttpContext.Session.SetInt32("userID", user.UserId);
            ViewBag.userID = HttpContext.Session.GetInt32("userID");
            // Kiểm tra và lấy URL từ session
            var returnUrl = HttpContext.Session.GetString("ReturnUrl");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Xóa ReturnUrl khỏi session
                HttpContext.Session.Remove("ReturnUrl");
                return Redirect(returnUrl);
            }

            if (user.Role == "Admin")
                // Chuyển hướng người dùng đến trang quản trị
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            // Chuyển hướng người dùng đến trang chủ của website
            return RedirectToAction("Index", "Home");
        }

        // Đăng nhập thất bại, chuyển hướng người dùng đến trang đăng nhập với thông báo lỗi
        ViewBag.ErrorMessage = "Invalid username or password.";
        return View("Index");
    }
}