﻿using BackEnd_Project.Models;
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
        // thêm logic để lưu dữ liệu đăng ký vào database
        var user = new User
        {
            Username = model.Username,
            Password = model.Password,
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role, 
            Address = model.Address,
            CreatedAt = DateTime.Now
        };
        _context.Users.Add(user);
        _context.SaveChanges();

      
        HttpContext.Session.SetInt32("userID", user.UserId);
        var returnUrl = HttpContext.Session.GetString("ReturnUrl");

        if (!string.IsNullOrEmpty(returnUrl))
        {
            // xóa ReturnUrl khỏi session
            HttpContext.Session.Remove("ReturnUrl");
            return Redirect(returnUrl);
        }

        // chuyển hướng sau khi đăng ký thành công
        return RedirectToAction("Index", "Home"); 
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.Username == username && u.Password == password);

        if (user != null)
        {
            // lưu thông tin userID vào session
            HttpContext.Session.SetInt32("userID", user.UserId);
            ViewBag.userID = HttpContext.Session.GetInt32("userID");
            // kiểm tra và lấy URL từ session
            var returnUrl = HttpContext.Session.GetString("ReturnUrl");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                // xóa ReturnUrl khỏi session
                HttpContext.Session.Remove("ReturnUrl");
                return Redirect(returnUrl);
            }

            if (user.Role == "Admin")
                // chuyển hướng người dùng đến trang quản trị
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            // chuyển hướng người dùng đến trang chủ của website
            return RedirectToAction("Index", "Home");
        }

        //ddăng nhập thất bại, chuyển hướng người dùng đến trang đăng nhập với thông báo lỗi
        ViewBag.ErrorMessage = "Invalid username or password.";
        return View("Index");
    }
}