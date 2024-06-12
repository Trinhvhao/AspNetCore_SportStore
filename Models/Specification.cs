using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BackEnd_Project.Models;

public class SpecificationDetail
{
    public string Attribute { get; set; }
    public string Detail { get; set; }
}

public class Specification
{
    public int SpecificationId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    [NotMapped]
    public List<SpecificationDetail> SpecificationDetails =>
        JsonConvert.DeserializeObject<List<SpecificationDetail>>(Value);
}