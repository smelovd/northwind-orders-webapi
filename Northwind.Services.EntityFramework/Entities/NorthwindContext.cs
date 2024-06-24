using Microsoft.EntityFrameworkCore;
using EntityCategory = Northwind.Services.EntityFramework.Entities.Category;
using EntityCustomer = Northwind.Services.EntityFramework.Entities.Customer;
using EntityEmployee = Northwind.Services.EntityFramework.Entities.Employee;
using EntityOrder = Northwind.Services.EntityFramework.Entities.Order;
using EntityOrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using EntityProduct = Northwind.Services.EntityFramework.Entities.Product;
using EntityShipper = Northwind.Services.EntityFramework.Entities.Shipper;
using EntitySupplier = Northwind.Services.EntityFramework.Entities.Supplier;

namespace Northwind.Services.EntityFramework.Entities;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<EntityCategory> Categories { get; set; }

    public DbSet<EntityCustomer> Customers { get; set; }

    public DbSet<EntityEmployee> Employees { get; set; }

    public DbSet<EntityOrder> Orders { get; set; }

    public DbSet<EntityOrderDetail> OrderDetails { get; set; }

    public DbSet<EntityProduct> Products { get; set; }

    public DbSet<EntityShipper> Shippers { get; set; }

    public DbSet<EntitySupplier> Suppliers { get; set; }
}
