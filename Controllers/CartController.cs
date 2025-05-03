using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class CartController : Controller
{
    private readonly SportStoreDb2Context _context;

    public CartController(SportStoreDb2Context context)
    {
        _context = context;
    }

    [HttpPost]
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1) // Mặc định số lượng là 1
    {
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null) return RedirectToAction("Index", "Register");

        // Kiểm tra xem sản phẩm đã có trong giỏ hàng của người dùng chưa
        var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            // Nếu có rồi, tăng số lượng
            cartItem.Quantity += quantity;
        }
        else
        {
            // Nếu chưa có, thêm sản phẩm mới vào giỏ hàng
            cartItem = new Cart
            {
                UserId = (int)userId,
                ProductId = productId,
                Quantity = quantity,
            };
            _context.Carts.Add(cartItem);
        }

        // Lưu thay đổi vào cơ sở dữ liệu
        await _context.SaveChangesAsync();

        // Chuyển hướng đến trang xem giỏ hàng
        return RedirectToAction("ViewCart");
    }




    public async Task<IActionResult> ViewCart()
    {
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null)
        {
            // người dùng chưa đăng nhập thì lưu URL hiện tại 
            HttpContext.Session.SetString("ReturnUrl", Url.Action("ViewCart", "Cart"));
            return RedirectToAction("Index", "Register");
        }

        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ThenInclude(p => p.Images)
            .ToListAsync();

        return View(cartItems);
    }

    // action để xoá sản phẩm khỏi giỏ hàng
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        // tìm sản phẩm trong giỏ hàng của người dùng
        var userId = HttpContext.Session.GetInt32("userID");
        var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            // xoá sản phẩm khỏi giỏ hàng
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("ViewCart");
    }
}