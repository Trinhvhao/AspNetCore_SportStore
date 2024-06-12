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
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null) return RedirectToAction("Index", "Register");

        var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cartItem = new Cart
            {
                UserId = (int)userId,
                ProductId = productId,
                Quantity = quantity
            };
            _context.Carts.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("ViewCart");
    }

    public async Task<IActionResult> ViewCart()
    {
        var userId = HttpContext.Session.GetInt32("userID");

        if (userId == null)
        {
            // Người dùng chưa đăng nhập, lưu URL hiện tại và chuyển hướng đến trang đăng nhập
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

    // Action để xoá sản phẩm khỏi giỏ hàng
    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        // Tìm sản phẩm trong giỏ hàng của người dùng
        var userId = HttpContext.Session.GetInt32("userID");
        var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            // Xoá sản phẩm khỏi giỏ hàng
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("ViewCart");
    }
}