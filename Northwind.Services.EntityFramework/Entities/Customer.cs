using System.ComponentModel.DataAnnotations;

namespace Northwind.Services.EntityFramework.Entities;
public class Customer
{
    public Customer(string customerId)
    {
        this.CustomerId = customerId;
    }

    [Key]
    [MaxLength(255)]
    public string? CustomerId { get; private set; }

    [MaxLength(255)]
    public string? CompanyName { get; init; } = default!;

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
}
