using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryADController : Controller
{
    private readonly SportStoreDb2Context _context;

    public CategoryADController(SportStoreDb2Context context)
    {
        _context = context;
    }

    // GET
    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    // GET: Admin/CategoryAD/Create
    public IActionResult Create()
    {
        return View();
    }

    //POST: Admin/CategoryAD/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    // POST: Admin/CategoryAD/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Category category)
    {
        if (id != category.CategoryId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    //DELETE
// GET: /Admin/Category/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = _context.Categories.FirstOrDefault(m => m.CategoryId == id);
        if (category == null) return NotFound();

        return RedirectToAction("Index");
    }

    // POST: /Admin/Product/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var category = _context.Categories.Find(id);
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }


// GET: Admin/CategoryAD/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();

        var category = _context.Categories
            .Include(c => c.Products) // Include() để lấy cả danh sách sản phẩm của Category
            .FirstOrDefault(m => m.CategoryId == id);

        if (category == null) return NotFound();

        return View(category);
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}