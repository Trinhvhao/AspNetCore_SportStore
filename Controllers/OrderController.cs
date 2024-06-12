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
        // Xác thực người dùng
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null)
            // Nếu người dùng chưa đăng nhập, chuyển hướng về trang đăng nhập hoặc trang chính
            return RedirectToAction("Index", "Register");

        // Lấy thông tin giỏ hàng của người dùng
        var cartItems = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        if (cartItems.Count == 0)
        {
            // Nếu giỏ hàng của người dùng trống, không thực hiện thanh toán và hiển thị thông báo cho người dùng
            TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            return RedirectToAction("ViewCart", "Cart");
        }

        // Tính tổng tiền
        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

        // Tạo đơn hàng
        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            OrderStatus = "Chờ xác nhận", // Gán giá trị mặc định cho orderStatus
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod,
            ShippingAddress = shippingAddress
        };

        // Lưu đơn hàng vào cơ sở dữ liệu
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Tạo chi tiết đơn hàng
        foreach (var cartItem in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price
            };

            // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
            _context.OrderDetails.Add(orderDetail);
        }

        // Xóa các sản phẩm trong giỏ hàng của người dùng sau khi checkout
        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        // Chuyển hướng đến trang thông báo thành công
        return RedirectToAction("OrderConfirm", new { orderId = order.OrderId });
    }

    public async Task<IActionResult> OrderConfirm(int orderId)
    {
        // Lấy thông tin đơn hàng dựa trên orderId
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.User) // Bao gồm thông tin người dùng
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        // Kiểm tra xem đơn hàng có tồn tại không
        if (order == null) return NotFound();

        // Truyền thông tin đơn hàng đến view
        return View(order);
    }
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        // Lấy đơn hàng từ cơ sở dữ liệu
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            // Trả về lỗi 404 nếu không tìm thấy đơn hàng
            return NotFound();
        }

        // Kiểm tra xem đơn hàng có đang ở trạng thái cho phép huỷ không
        if (order.OrderStatus != "Chờ xác nhận")
        {
            // Trả về lỗi nếu đơn hàng không thể được huỷ
            return BadRequest("This order cannot be canceled.");
        }

        // Huỷ đơn hàng bằng cách đặt trạng thái là "Canceled"
        order.OrderStatus = "Đã Huỷ";

        // Lưu thay đổi vào cơ sở dữ liệu
        await _context.SaveChangesAsync();

        // Chuyển hướng người dùng đến trang chủ hoặc trang lịch sử đơn hàng
        return RedirectToAction("Index", "User");
    }
}