using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class OrderController : Controller
{
    private readonly SportStoreDb2Context _context;

    public OrderController(SportStoreDb2Context context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(string shippingMethod, string paymentMethod, string shippingAddress)
    {
        // xác thụực
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null)
            // chưa đăng nhập thì chuyển đến đăng nhập
            return RedirectToAction("Index", "Register");

        // lấy thông tin từ giỏ hàng
        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        if (cartItems.Count == 0)
        {
           
            TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            return RedirectToAction("ViewCart", "Cart");
        }

        // tính tổng tiền
        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

        // tạo đơn hàng
        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            OrderStatus = "Chờ xác nhận", 
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod,
            ShippingAddress = shippingAddress
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        // chi tiết đơn hàng
        foreach (var cartItem in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price
            };
            _context.OrderDetails.Add(orderDetail);
        }
        
        // xóa các sản phẩm trong giỏ hàng 
        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        return RedirectToAction("OrderConfirm", new { orderId = order.OrderId });
    }

    public async Task<IActionResult> OrderConfirm(int orderId)
    {
        // lấy thông tin đơn hàng dựa trên orderId
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.User) 
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        
        if (order == null) return NotFound();
        
        return View(order);
    }
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        // lấy đơn hàng từ cơ sở dữ liệu
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return NotFound();
        }

        // Kkểm tra xem đơn hàng có đang ở trạng thái cho phép huỷ không
        if (order.OrderStatus != "Chờ xác nhận")
        {
            return BadRequest("This order cannot be canceled.");
        }
        // huỷ đơn hàng bằng cách đặt trạng thái là "Canceled"
        order.OrderStatus = "Đã Huỷ";
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index", "User");
    }
}