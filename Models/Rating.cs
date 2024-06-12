namespace BackEnd_Project.Models;

public class Rating
{
    public int RatingId { get; set; }

    public int Value { get; set; }

    public DateTime CreatedAt { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}