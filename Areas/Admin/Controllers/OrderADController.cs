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
    
    public IActionResult Index()
    {
        var orders = _context.Orders.ToList();
        return View(orders);
    }

    // chi tiet

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound("Order ID is missing.");

        // lay thong tin dua tren order
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.User) // Bao gồm thông tin người dùng
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null) return NotFound("Order not found.");
        
        return View(order);
    }
    
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

    
    // GET: /Order/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();

        var order = _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null) return NotFound();

        return View(order);
    }

    // POST: /Order/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.OrderId) return NotFound();

                _context.Update(order);
                await _context.SaveChangesAsync();
         
            return RedirectToAction(nameof(Index));
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
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        if (order.OrderStatus == "Chờ xác nhận")
        {
            order.OrderStatus = "Đang giao";
            _context.Update(order);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Admin");
    }

    // Hiển thị danh sách đơn hàng theo trạng thái
    public IActionResult OrdersByStatus(string status)
    {
        var orders = _context.Orders.Where(o => o.OrderStatus == status).ToList();
        return View("Index", orders);
    }

}

