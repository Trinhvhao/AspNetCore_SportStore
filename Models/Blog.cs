namespace BackEnd_Project.Models;

public class Blog
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public int? UserId { get; set; }

    public string? Tags { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? User { get; set; }
}