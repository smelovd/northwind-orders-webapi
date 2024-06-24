using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityOrder = Northwind.Services.EntityFramework.Entities.Order;
using EntityProduct = Northwind.Services.EntityFramework.Entities.Product;

namespace Northwind.Services.EntityFramework.Entities;
public class OrderDetail
{
    public OrderDetail(long orderId)
    {
        this.OrderId = orderId;
    }

    public long OrderId { get; private set; }

    [ForeignKey("OrderId")]
    public EntityOrder Order { get; set; } = null!;

    [Key]
    public long ProductId { get; set; }

    [ForeignKey("ProductId")]
    public EntityProduct Product { get; set; } = null!;

    public double UnitPrice { get; set; }

    public long Quantity { get; set; }

    public double Discount { get; set; }
}
