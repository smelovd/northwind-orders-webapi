using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using EntityCustomer = Northwind.Services.EntityFramework.Entities.Customer;
using EntityOrder = Northwind.Services.EntityFramework.Entities.Order;
using EntityOrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryProduct = Northwind.Services.Repositories.Product;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext context;

    public OrderRepository(NorthwindContext context)
    {
        this.context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        ValidateOrderId(orderId);
        EntityOrder order = await this.GetEntityOrders()
            .FirstOrDefaultAsync(o => o!.OrderId == orderId) ?? throw new OrderNotFoundException();

        return MapToRepositoryOrder(order);
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        ValidateGetOrdersAsyncData(skip, count);

        var orders = await this.GetEntityOrders()
            .Skip(skip)
            .Take(count)
            .ToListAsync();
        List<RepositoryOrder> repositoryOrders = new List<RepositoryOrder>();
        foreach (EntityOrder? order in orders)
        {
            if (order != null)
            {
                repositoryOrders.Add(MapToRepositoryOrder(order));
            }
        }

        return repositoryOrders;
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        ValidateAddOrderData(order);

        EntityOrder entityOrder = new EntityOrder()
        {
            EmployeeId = order.Employee.Id,
            CustomerId = order.Customer.Code.Code,
            Freight = order.Freight,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShipAddress = order.ShippingAddress.Address,
            ShipCity = order.ShippingAddress.City,
            ShipCountry = order.ShippingAddress.Country,
            ShipName = order.ShipName,
            ShippedDate = order.ShippedDate,
        };
        this.context.Orders.Add(entityOrder);

        foreach (var od in order.OrderDetails)
        {
            if ((await this.context.Products.FirstOrDefaultAsync(p => p.ProductId == od.Product.Id)) == null ||
                od.UnitPrice < 0 || od.Quantity <= 0 || od.Discount < 0 || od.Discount > 1)
            {
                throw new RepositoryException();
            }

            try
            {
                var entityOrderDetail = new EntityOrderDetail(entityOrder.OrderId)
                {
                    ProductId = od.Product.Id, UnitPrice = od.UnitPrice, Quantity = od.Quantity, Discount = od.Discount,
                };
                entityOrder.OrderDetails.Add(entityOrderDetail);
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            catch (RepositoryException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        await this.context.SaveChangesAsync();
        return entityOrder.OrderId;
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        EntityOrder order = await this.context.Orders
            .Include<EntityOrder, List<EntityOrderDetail>>(order => order.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == orderId) ?? throw new OrderNotFoundException();

        order.OrderDetails.ForEach(orderDetail => this.context.OrderDetails.Remove(orderDetail));
        this.context.Orders.Remove(order);

        await this.context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        ValidateOrder(order);

        EntityOrder entityOrder = await this.context.Orders
            .Include<EntityOrder, List<EntityOrderDetail>>(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == order.Id) ?? throw new OrderNotFoundException();

        entityOrder.EmployeeId = order.Employee.Id;
        entityOrder.CustomerId = order.Customer.Code.Code;
        entityOrder.Freight = order.Freight;
        entityOrder.OrderDate = order.OrderDate;
        entityOrder.RequiredDate = order.RequiredDate;
        entityOrder.ShipAddress = order.ShippingAddress.Address;
        entityOrder.ShipCity = order.ShippingAddress.City;
        entityOrder.ShipCountry = order.ShippingAddress.Country;
        entityOrder.ShipCountry = order.ShippingAddress.Country;
        entityOrder.ShipRegion = order.ShippingAddress.Region;
        entityOrder.ShipPostalCode = order.ShippingAddress.PostalCode;
        entityOrder.ShipName = order.ShipName;
        entityOrder.ShipVia = order.Shipper.Id;

        if (order.ShippedDate != null)
        {
            entityOrder.ShippedDate = order.ShippedDate;
        }

        var newOrderDetailIds = order.OrderDetails.Select(od => od.Product.Id).ToList();

        var orderDetailsToRemove = entityOrder.OrderDetails
            .Where(od => !newOrderDetailIds.Contains(od.ProductId))
            .ToList();
        this.context.OrderDetails.RemoveRange(orderDetailsToRemove);

        foreach (var od in order.OrderDetails)
        {
            var existingOrderDetail = entityOrder.OrderDetails
                .FirstOrDefault(eod => eod.ProductId == od.Product.Id);

            if (existingOrderDetail != null)
            {
                existingOrderDetail.UnitPrice = od.UnitPrice;
                existingOrderDetail.Quantity = od.Quantity;
                existingOrderDetail.Discount = od.Discount;
            }
            else
            {
                var newOrderDetail = new EntityOrderDetail(order.Id)
                {
                    ProductId = od.Product.Id, UnitPrice = od.UnitPrice, Quantity = od.Quantity, Discount = od.Discount,
                };
                entityOrder.OrderDetails.Add(newOrderDetail);
            }
        }

        this.context.Orders.Update(entityOrder);

        await this.context.SaveChangesAsync();
    }

    private static void ValidateOrder(RepositoryOrder order)
    {
        if (order == null)
        {
            throw new ArgumentException(null, nameof(order));
        }
    }

    private static RepositoryOrder MapToRepositoryOrder(EntityOrder order)
    {
        RepositoryOrder repositoryOrder = new RepositoryOrder(order.OrderId)
        {
            Customer = new RepositoryCustomer(new CustomerCode(order.CustomerId!))
                    {
                        CompanyName = order.Customer!.CompanyName!,
                    },
            Employee = new RepositoryEmployee(order.Employee!.EmployeeId)
                    {
                        Country = order.Employee.Country,
                        FirstName = order.Employee.FirstName,
                        LastName = order.Employee.LastName,
                    },
            Freight = order.Freight,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShipName = order.ShipName,
            ShippedDate = order.ShippedDate,
            Shipper = new RepositoryShipper(order.Shipper!.ShipperId) { CompanyName = order.Shipper.CompanyName, },
            ShippingAddress =
                new ShippingAddress(
                    order.ShipAddress,
                    order.ShipCity,
                    order.ShipRegion,
                    order.ShipPostalCode,
                    order.ShipCountry),
        };
        order.OrderDetails.ForEach(orderDetail =>
        {
            if (orderDetail.Product.Supplier.CompanyName != null)
            {
                repositoryOrder.OrderDetails.Add(new RepositoryOrderDetail(repositoryOrder)
                {
                    Discount = orderDetail.Discount,
                    Product = new RepositoryProduct(orderDetail.ProductId)
                    {
                        CategoryId = orderDetail.Product.Category.CategoryId,
                        Category = orderDetail.Product.Category.CategoryName,
                        SupplierId = orderDetail.Product.Supplier.SupplierId,
                        Supplier = orderDetail.Product.Supplier.CompanyName,
                        ProductName = orderDetail.Product.ProductName,
                    },
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                });
            }
        });
        return repositoryOrder;
    }

    private static void ValidateGetOrdersAsyncData(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
    }

    private static void ValidateAddOrderData(RepositoryOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
    }

    private static void ValidateOrderId(long orderId)
    {
        ArgumentNullException.ThrowIfNull(orderId);
    }

    private IIncludableQueryable<EntityOrder?, Category> GetEntityOrders()
    {
        return this.context.Orders
            .Include<EntityOrder, EntityCustomer>(o => o.Customer!)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
            .ThenInclude(o => o.Product)
            .ThenInclude(o => o.Supplier)
            .Include(o => o.OrderDetails)
            .ThenInclude(o => o.Product)
            .ThenInclude(o => o.Category);
    }
}
