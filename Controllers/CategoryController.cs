using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BackEnd_Project.Controllers;

public class CategoryController : Controller
{
    private readonly SportStoreDb2Context _context;

    public CategoryController(SportStoreDb2Context context)
    {
        _context = context;
    }

    private CategoryViewModel GetViewModel(int? categoryId = null, string query = null, int page = 1, int pageSize = 6)
    {
        var sidebarData = _context.Categories.ToList();
        IPagedList<Product> productsWithImage;

        if (categoryId.HasValue)
        {
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .Where(p => p.CategoryId == categoryId.Value)
                .ToPagedList(page, pageSize);
        }
        else if (!string.IsNullOrEmpty(query))
        {
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .Where(p => p.Name.Contains(query))
                .ToPagedList(page, pageSize);
        }
        else
        {
            productsWithImage = _context.Products
                .Include(p => p.Images)
                .ToPagedList(page, pageSize);
        }

        return new CategoryViewModel
        {
            SidebarCategories = sidebarData,
            ProductCategory = productsWithImage
        };
    }

    public IActionResult Index(int? categoryId = null, string query = null, int page = 1)
    {
        var viewModel = GetViewModel(categoryId, query, page);
        return View("CategoryProduct", viewModel);
    }

    [HttpGet]
    public IActionResult ProductByCategory(int categoryId, int page = 1)
    {
        var viewModel = GetViewModel(categoryId, null, page);
        return View("CategoryProduct", viewModel);
    }
}