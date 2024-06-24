using System.ComponentModel.DataAnnotations;

namespace Northwind.Services.EntityFramework.Entities;
public class Employee
{
    public Employee(long employeeId)
    {
        this.EmployeeId = employeeId;
    }

    [Key]
    public long EmployeeId { get; private set; }

    [MaxLength(255)]
    public string LastName { get; init; } = default!;

    [MaxLength(255)]
    public string FirstName { get; init; } = default!;

    [MaxLength(255)]
    public string Title { get; set; } = null!;

    [MaxLength(255)]
    public string TitleOfCourtesy { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public DateTime HireDate { get; set; }

    [MaxLength(255)]
    public string Address { get; set; } = null!;

    [MaxLength(255)]
    public string City { get; set; } = null!;

    [MaxLength(255)]
    public string? Region { get; set; }

    [MaxLength(255)]
    public string PostalCode { get; set; } = null!;

    [MaxLength(255)]
    public string Country { get; set; } = null!;

    [MaxLength(255)]
    public string HomePhone { get; set; } = null!;

    [MaxLength(255)]
    public string Extension { get; set; } = null!;

    [MaxLength(255)]
    public string Notes { get; set; } = null!;

    [MaxLength(255)]
    public string ReportsTo { get; set; } = null!;

    [MaxLength(255)]
    public string PhotoPath { get; set; } = null!;
}
