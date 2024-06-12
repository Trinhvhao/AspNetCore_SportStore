namespace BackEnd_Project.Models;

public class UserOrderViewModel
{
    public User User { get; set; }
    public List<Order> Orders { get; set; }
}