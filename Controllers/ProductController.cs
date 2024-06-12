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
        var products = _context.Products.ToList(); 
        return View(products); 
    }

    // GET: /Product/Details/5
    // để hiển thị chi tiết sản phẩm dựa trên productId
    public IActionResult ProductDetail(int productId)
    {
        var product = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .FirstOrDefault(p => p.ProductId == productId);


        if (product == null) return NotFound(); // trả về lỗi nếu không tìm thấy

        // lấy dữ liệu từ cột Size và chia thành các phần dựa trên dấu phẩy
        var sizeArray = product.Size?.Split(',');

        // chuyển đổi mảng thành danh sách (List) để sử dụng trong view
        var sizesList = sizeArray?.ToList();

        // đặt giá trị vào ViewBag
        ViewBag.SizesList = sizesList;

        // lấy danh sách comment của sản phẩm
        var comments = _context.Comments
            .Where(c => c.ProductId == productId)
            .ToList();
        ViewBag.Comments = comments;

        // lấy danh sách người dùng và lưu vào ViewBag
        var users = _context.Users.ToDictionary(u => u.UserId, u => u.FullName);
        ViewBag.Users = users;


        // lấy thông tin Specifications của sản phẩm
        var specification = _context.Specifications
            .Where(s => s.ProductId == productId)
            .ToList();
        ViewBag.Specifications = specification;

        // trả về view "ProductDetail" với dữ liệu của sản phẩm và danh sách size
        return View("ProductDetail", product);
    }

    [HttpPost]
    public IActionResult CreateComment(int productId, string content)
    {
        var userId = HttpContext.Session.GetInt32("userID");
        if (userId == null)
        {
            HttpContext.Session.SetString("ReturnUrl", Url.Action("ProductDetail", "Product", new { productId }));
            return RedirectToAction("Index", "Register");
        }

        //  nếu người dùng đã đăng nhập, tiến hành tạo comment
        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.Now,
            ProductId = productId,
            UserId = userId.Value
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();
        
        return RedirectToAction("ProductDetail", new { productId });
    }

    // tìm kiếm sản phẩm
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        return RedirectToAction("Index", "Category", new { query });
    }
}