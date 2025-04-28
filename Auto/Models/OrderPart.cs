using Auto.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderPart
{
    public int OrderPartId { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int PartId { get; set; }
    public Part? Part { get; set; }

    [Required(ErrorMessage = "Количество обязательно.")]
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0.")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    public int? CarModelId { get; set; } 
    public CarModel? CarModel { get; set; }
    [NotMapped]
    public string? PartName { get; set; }
}
