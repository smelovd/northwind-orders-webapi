using System.ComponentModel.DataAnnotations;

namespace Northwind.Services.EntityFramework.Entities;
public class Shipper
{
    public Shipper(long shipperId)
    {
        this.ShipperId = shipperId;
    }

    [Key]
    public long ShipperId { get; private set; }

    [MaxLength(255)]
    public string CompanyName { get; set; } = null!;

    [MaxLength(255)]
    public string Phone { get; set; } = null!;
}
