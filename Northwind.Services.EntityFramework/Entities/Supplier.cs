using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;
[Table("Suppliers")]
public class Supplier
{
    public Supplier(long supplierId)
    {
        this.SupplierId = supplierId;
    }

    [Key]
    public long SupplierId { get; set; }

    [MaxLength(255)]
    public string? CompanyName { get; set; }

    [MaxLength(255)]
    public string? ContactName { get; set; }

    [MaxLength(255)]
    public string? ContactTitle { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(255)]
    public string? City { get; set; }

    [MaxLength(255)]
    public string? Region { get; set; }

    [MaxLength(255)]
    public string? PostalCode { get; set; }

    [MaxLength(255)]
    public string? Country { get; set; }

    [MaxLength(255)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Fax { get; set; }

    [MaxLength(255)]
    public string? HomePage { get; set; }
}
