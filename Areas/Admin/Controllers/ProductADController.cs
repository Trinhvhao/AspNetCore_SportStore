using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Import model Product và DbContext tương ứng

namespace BackEnd_Project.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductADController : Controller
{
    private readonly SportStoreDb2Context _context;
    private readonly IWebHostEnvironment _env;

    public ProductADController(IWebHostEnvironment env, SportStoreDb2Context context)
    {
        _env = env;
        _context = context;
    }

    // GET: /Admin/Product
    public IActionResult Index()
    {
        var products = _context.Products.ToList();
        return View(products);
    }

    // GET: /Admin/Product/Create
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList(); // Lấy danh sách categories để hiển thị trong dropdown
        return View();
    }

// POST: /Admin/Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        // Thêm sản phẩm mới vào DbContext và lưu thay đổi
        _context.Products.Add(product);
        await _context.SaveChangesAsync(); // Sử dụng await ở đây
        ViewBag.Categories = _context.Categories.ToList(); // Lấy lại danh sách categories để hiển thị trong dropdown
        // Chuyển hướng đến action Index sau khi tạo sản phẩm thành công
        return RedirectToAction(nameof(Index));
    }


    // GET: /Admin/Product/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = _context.Products.Find(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // POST: /Admin/Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Product product)
    {
        if (id != product.ProductId) return NotFound();
        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Product/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = _context.Products.FirstOrDefault(m => m.ProductId == id);
        if (product == null) return NotFound();

        return RedirectToAction("Index");
    }

    // POST: /Admin/Product/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        _context.Products.Remove(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Product/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();

        var product = _context.Products
            .Include(p => p.Images)
            .FirstOrDefault(m => m.ProductId == id);
        if (product == null) return NotFound();

        return View(product);
    }


    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }

    [HttpGet]
    public IActionResult AddImage(int id)
    {
        var image = new Image { ProductId = id };
        return View(image);
    }

    [HttpPost]
    public IActionResult AddImage(Image image)
    {
        var fileName = Path.GetFileNameWithoutExtension(image.ImageFile.FileName); //Lấy tên file ảnh
        var extension = Path.GetExtension(image.ImageFile.FileName); //Lấy phần path ảnh
        var uniqueFileName = fileName + "_" + Guid.NewGuid() + extension; //Tạo ra url từ fileName + Path
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images"); //Xác định mục tiêu lưu ảnh
        var filePath = Path.Combine(uploadsFolder, uniqueFileName); //Tạo ra url từ thư mục + url(fileName + Path)

        // Lưu file vào thư mục
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image.ImageFile.CopyTo(fileStream);
        }

        // Lưu đường dẫn tương đối vào thuộc tính ImagePath của image
        image.ImagePath = "/images/" + uniqueFileName;
        _context.Images.Add(image);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // POST: /Admin/Product/DeleteImage/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteImage(int id)
    {
        var image = _context.Images.Find(id);
        if (image == null) return NotFound();

        // Xoá ảnh từ thư mục images
        var imagePath = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
        if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

        // Xoá ảnh từ cơ sở dữ liệu
        _context.Images.Remove(image);
        _context.SaveChanges();
        return RedirectToAction("Details", new { id = image.ProductId });
    }

    // GET: /Admin/Specification/Create
    public IActionResult CreateSpecification(int productId)
    {
        ViewBag.ProductID = productId;
        TempData["ProductId"] = productId;
        return View();
    }

    // POST: /Admin/Specification/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSpecification(int productId, Specification specification)
    {
        specification.ProductId = productId;
        _context.Specifications.Add(specification);
        await _context.SaveChangesAsync();
        ViewBag.ProductId = productId;
        return RedirectToAction(nameof(Details), new { id = productId });
    }

    // GET: /Admin/Specification/Edit/5
    public async Task<IActionResult> EditSpecification(int? id)
    {
        if (id == null) return NotFound();

        var specification = await _context.Specifications.FindAsync(id);
        if (specification == null) return NotFound();

        ViewBag.ProductId = specification.ProductId;
        return View(specification);
    }

    // POST: /Admin/Specification/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSpecification(int id, Specification specification)
    {
        if (id != specification.SpecificationId) return NotFound();

        _context.Update(specification);
        await _context.SaveChangesAsync();
        ViewBag.ProductId = specification.ProductId;
        return RedirectToAction(nameof(Details), new { id = specification.ProductId });
    }

    // GET: /Admin/Specification/Delete/5
    public async Task<IActionResult> DeleteSpecification(int? id)
    {
        if (id == null) return NotFound();

        var specification = await _context.Specifications
            .Include(s => s.Product)
            .FirstOrDefaultAsync(m => m.SpecificationId == id);
        if (specification == null) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/Specification/Delete/5
    [HttpPost]
    [ActionName("DeleteSpecification")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSpecificationConfirmed(int id)
    {
        var specification = await _context.Specifications.FindAsync(id);
        var productId = specification.ProductId;
        _context.Specifications.Remove(specification);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = productId });
    }

// Action để xoá comment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        // Tìm comment trong cơ sở dữ liệu
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return NotFound(); // Trả về lỗi 404 nếu không tìm thấy comment

        // Xoá comment khỏi cơ sở dữ liệu
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { productId = comment.ProductId });
    }
}