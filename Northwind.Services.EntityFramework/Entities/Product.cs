using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityCategory = Northwind.Services.EntityFramework.Entities.Category;
using EntitySupplier = Northwind.Services.EntityFramework.Entities.Supplier;

namespace Northwind.Services.EntityFramework.Entities;

[Table("Products")]
public class Product
{
    public Product(long productId)
    {
        this.ProductId = productId;
    }

    [Key]
    public long ProductId { get; private set; }

    [MaxLength(255)]
    public string ProductName { get; init; } = default!;

    public long SupplierId { get; init; }

    [ForeignKey("SupplierId")]
    public EntitySupplier Supplier { get; init; } = null!;

    public long CategoryId { get; init; }

    [ForeignKey("CategoryId")]
    public EntityCategory Category { get; init; } = null!;

    [MaxLength(255)]
    public string QuantityPerUnit { get; init; } = null!;

    public double UnitPrice { get; init; }

    public long UnitsInStock { get; init; }

    public long UnitsOnOrder { get; init; }

    public long ReorderLevel { get; init; }

    public long Discontinued { get; init; }
}
