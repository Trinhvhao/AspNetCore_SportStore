using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd_Project.Controllers;

public class CategoryController : Controller
{
    private readonly SportStoreDb2Context _context;

    public CategoryController(SportStoreDb2Context context)
    {
        _context = context;
    }

    private CategoryViewModel GetViewModel(int? categoryId = null, string query = null)
    {
        var sidebarData = _context.Categories.ToList();
        List<Product> productsWithImage;


        if (categoryId.HasValue)
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .Where(p => p.CategoryId == categoryId.Value)
                .ToList();
        else if (!string.IsNullOrEmpty(query))
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .Where(p => p.Name.Contains(query))
                .ToList();
        else
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .ToList();

        return new CategoryViewModel
        {
            SidebarCategories = sidebarData,
            ProductCategory = productsWithImage
        };
    }

    public IActionResult Index(int? categoryId = null, string query = null)
    {
        var viewModel = GetViewModel(categoryId, query);
        return View("CategoryProduct", viewModel);
    }

    [HttpGet]
    public IActionResult ProductByCategory(int categoryId)
    {
        var viewModel = GetViewModel(categoryId);
        return View("CategoryProduct", viewModel);
    }
}