namespace BackEnd_Project.Models;

public class Shipment
{
    public int ShipmentId { get; set; }

    public int UserId { get; set; }

    public string ShipmentAddress { get; set; } = null!;

    public DateTime? ShipmentDate { get; set; }

    public string? ShipmentStatus { get; set; }

    public virtual User User { get; set; } = null!;
}