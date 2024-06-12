using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class ProductController : Controller
{
    private readonly SportStoreDb2Context _context = new();

    private readonly IWebHostEnvironment _env;

    // GET
    public IActionResult Index()
    {
        var products = _context.Products.ToList(); // Lấy danh sách sản phẩm từ cơ sở dữ liệu
        return View(products); // Trả về view Index và chuyển danh sách sản phẩm sang view
    }

    // GET: /Product/Details/5
    // Action để hiển thị chi tiết sản phẩm dựa trên productId
    public IActionResult ProductDetail(int productId)
    {
        // Lấy thông tin chi tiết của sản phẩm dựa trên productId
        var product = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == productId);


        if (product == null) return NotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm

        // Lấy dữ liệu từ cột Size và chia thành các phần dựa trên dấu phẩy
        var sizeArray = product.Size?.Split(',');

        // Chuyển đổi mảng thành danh sách (List) để sử dụng trong view
        var sizesList = sizeArray?.ToList();

        // Đặt giá trị vào ViewBag
        ViewBag.SizesList = sizesList;

        // Lấy danh sách comment của sản phẩm
        var comments = _context.Comments
            .Where(c => c.ProductId == productId)
            .ToList();
        ViewBag.Comments = comments;

        // Lấy danh sách người dùng và lưu vào ViewBag
        var users = _context.Users.ToDictionary(u => u.UserId, u => u.FullName);
        ViewBag.Users = users;


        // Lấy thông tin Specifications của sản phẩm
        var specification = _context.Specifications
            .Where(s => s.ProductId == productId)
            .ToList();
        ViewBag.Specifications = specification;

        // Trả về view "ProductDetail" với dữ liệu của sản phẩm và danh sách size
        return View("ProductDetail", product);
    }

    [HttpPost]
    public IActionResult CreateComment(int productId, string content)
    {
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null)
        {
            // Người dùng chưa đăng nhập, lưu URL hiện tại và chuyển hướng đến trang đăng nhập
            HttpContext.Session.SetString("ReturnUrl", Url.Action("ProductDetail", "Product", new { productId }));
            return RedirectToAction("Index", "Register");
        }

        // Người dùng đã đăng nhập, tiến hành tạo comment
        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            ProductId = productId,
            UserId = userId.Value
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        // Chuyển hướng người dùng trở lại trang chi tiết sản phẩm
        return RedirectToAction("ProductDetail", new { productId });
    }

    // Action để tìm kiếm sản phẩm
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        return RedirectToAction("Index", "Category", new { query });
    }
}