namespace BackEnd_Project.Models;

public class CategoryViewModel
{
    public IEnumerable<Category> SidebarCategories { get; set; }
    public IEnumerable<Product> ProductAll { get; set; }
    public IEnumerable<Product> ProductCategory { get; set; }
    public bool IsCategorySelected { get; set; }
}