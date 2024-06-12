namespace BackEnd_Project.Models;

public class ProductHomeView
{
    public IEnumerable<Product> LatestProducts { get; set; }
    public IEnumerable<Product> UpcomingProducts { get; set; }
}