using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityCustomer = Northwind.Services.EntityFramework.Entities.Customer;
using EntityEmployee = Northwind.Services.EntityFramework.Entities.Employee;
using EntityOrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using EntityShipper = Northwind.Services.EntityFramework.Entities.Shipper;

namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    public Order(long orderId)
    {
        this.OrderId = orderId;
    }

    public Order()
    {
    }

    public long OrderId { get; private set; }

    [MaxLength(255)]
    public string? CustomerId { get; set; }

    [ForeignKey("CustomerId")]
    public EntityCustomer? Customer { get; set; }

    public long? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    public EntityEmployee? Employee { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public long? ShipVia { get; set; }

    [ForeignKey("ShipVia")]
    public EntityShipper? Shipper { get; set; }

    public double Freight { get; set; }

    [MaxLength(255)]
    public string ShipName { get; set; } = default!;

    [MaxLength(255)]
    public string ShipAddress { get; set; } = default!;

    [MaxLength(255)]
    public string ShipCity { get; set; } = null!;

    [MaxLength(255)]
    public string? ShipRegion { get; set; }

    [MaxLength(255)]
    public string ShipPostalCode { get; set; } = null!;

    [MaxLength(255)]
    public string ShipCountry { get; set; } = null!;

    public List<EntityOrderDetail> OrderDetails { get; init; } = new List<EntityOrderDetail>();
}
