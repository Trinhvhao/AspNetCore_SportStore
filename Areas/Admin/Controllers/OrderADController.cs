using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Areas.Admin.Controllers;

[Area("Admin")]
public class OrderADController : Controller
{
    private readonly SportStoreDb2Context _context;

    public OrderADController(SportStoreDb2Context context)
    {
        _context = context;
    }

    // Hiển thị danh sách đơn hàng
    public IActionResult Index()
    {
        var orders = _context.Orders.ToList();
        return View(orders);
    }

    // Chi tiết đơn hàng

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound("Order ID is missing.");

        // Lấy thông tin đơn hàng dựa trên orderId
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.User) // Bao gồm thông tin người dùng
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null) return NotFound("Order not found.");

        // Truyền thông tin đơn hàng đến view
        return View(order);
    }

    // Tạo đơn hàng mới
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        _context.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Sửa đơn hàng
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Quantity,Price,TotalPrice")] Order order)
    {
        if (id != order.OrderId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // Action để lấy ra các đơn hàng được tạo vào ngày hiện tại
    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.OrderId == id);
    }
}