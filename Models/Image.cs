using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd_Project.Models;

public class Image
{
    [NotMapped] public IFormFile ImageFile { get; set; } = null!;

    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string? ImagePath { get; set; }

    public virtual Product Product { get; set; } = null!;
}