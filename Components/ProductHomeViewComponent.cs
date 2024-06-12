using BackEnd_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductHomeViewComponent : ViewComponent
{
    private readonly SportStoreDb2Context _context;

    public ProductHomeViewComponent(SportStoreDb2Context context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Lấy 6 sản phẩm mới nhất
        var latestProducts = await _context.Products
            .Take(8)
            .ToListAsync();

        // Lấy 6 sản phẩm sắp ra mắt không trùng với sản phẩm mới nhất
        var latestProductIds = latestProducts.Select(p => p.ProductId).ToList();
        var upcomingProducts = await _context.Products
            .Where(p => !latestProductIds.Contains(p.ProductId))
            .Take(8)
            .ToListAsync();

        var model = new ProductHomeView
        {
            LatestProducts = latestProducts,
            UpcomingProducts = upcomingProducts
        };

        return View(model);
    }
}