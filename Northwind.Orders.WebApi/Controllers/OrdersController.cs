using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Orders.WebApi.Models;
using Northwind.Services.Repositories;
using ModelCustomer = Northwind.Orders.WebApi.Models.Customer;
using ModelEmployee = Northwind.Orders.WebApi.Models.Employee;
using ModelShipper = Northwind.Orders.WebApi.Models.Shipper;
using ModelShippingAddress = Northwind.Orders.WebApi.Models.ShippingAddress;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryProduct = Northwind.Services.Repositories.Product;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;
using RepositoryShippingAddress = Northwind.Services.Repositories.ShippingAddress;

namespace Northwind.Orders.WebApi.Controllers;

[ApiController]
[Route("/api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository orderRepository;
    private readonly ILogger<OrdersController> log;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        this.orderRepository = orderRepository;
        this.log = logger;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<FullOrder>> GetOrderAsync(long orderId)
    {
        Console.WriteLine(this.log.IsEnabled(LogLevel.Critical));
        try
        {
            var order = await this.orderRepository.GetOrderAsync(orderId);
            var fullOrder = new FullOrder()
            {
                Customer =
                    new ModelCustomer() { Code = order.Customer.Code.Code, CompanyName = order.Customer.CompanyName, },
                Employee = new ModelEmployee()
                {
                    Id = order.Employee.Id,
                    Country = order.Employee.Country,
                    FirstName = order.Employee.FirstName,
                    LastName = order.Employee.LastName,
                },
                Freight = order.Freight,
                Id = order.Id,
                OrderDate = order.OrderDate,
                ShippedDate = order.ShippedDate,
                RequiredDate = order.RequiredDate,
                Shipper = new ModelShipper() { CompanyName = order.Shipper.CompanyName, Id = order.Shipper.Id, },
                ShipName = order.ShipName,
                ShippingAddress = new ModelShippingAddress()
                {
                    Address = order.ShippingAddress.Address,
                    City = order.ShippingAddress.City,
                    Country = order.ShippingAddress.Country,
                    PostalCode = order.ShippingAddress.PostalCode,
                    Region = order.ShippingAddress.Region,
                },
                OrderDetails = new List<FullOrderDetail>(),
            };
            foreach (var orderDetail in order.OrderDetails)
            {
                fullOrder.OrderDetails.Add(new FullOrderDetail()
                {
                    ProductId = orderDetail.Product.Id,
                    ProductName = orderDetail.Product.ProductName,
                    CategoryId = orderDetail.Product.CategoryId,
                    CategoryName = orderDetail.Product.Category,
                    SupplierId = orderDetail.Product.SupplierId,
                    SupplierCompanyName = orderDetail.Product.Supplier,
                    Discount = orderDetail.Discount,
                    UnitPrice = orderDetail.UnitPrice,
                    Quantity = orderDetail.Quantity,
                });
            }

            return new OkObjectResult(fullOrder);
        }
        catch (OrderNotFoundException e)
        {
            Console.WriteLine(e);
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(500);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BriefOrder>>> GetOrdersAsync(int? skip = 0, int? count = 10)
    {
        try
        {
            IList<RepositoryOrder> repositoryOrders =
                await this.orderRepository.GetOrdersAsync(skip ?? 0, count ?? 10);
            List<BriefOrder> briefOrders = new List<BriefOrder>();
            foreach (var order in repositoryOrders)
            {
                var briefOrder = new BriefOrder()
                {
                    Id = order.Id,
                    CustomerId = order.Customer.Code.Code,
                    EmployeeId = order.Employee.Id,
                    Freight = order.Freight,
                    OrderDate = order.OrderDate,
                    ShippedDate = order.ShippedDate,
                    RequiredDate = order.RequiredDate,
                    ShipperId = order.Shipper.Id,
                    ShipName = order.ShipName,
                    ShipAddress = order.ShippingAddress.Address,
                    ShipCity = order.ShippingAddress.City,
                    ShipCountry = order.ShippingAddress.Country,
                    ShipPostalCode = order.ShippingAddress.PostalCode,
                    ShipRegion = order.ShippingAddress.Region,
                    OrderDetails = new List<BriefOrderDetail>(),
                };
                foreach (var orderDetail in order.OrderDetails)
                {
                    briefOrder.OrderDetails.Add(new BriefOrderDetail()
                    {
                        ProductId = orderDetail.Product.Id,
                        Discount = orderDetail.Discount,
                        UnitPrice = orderDetail.UnitPrice,
                        Quantity = orderDetail.Quantity,
                    });
                }

                briefOrders.Add(briefOrder);
            }

            return new OkObjectResult(briefOrders);
        }
        catch (OrderNotFoundException e)
        {
            Console.WriteLine(e);
            return new NotFoundResult();
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(e);
            return new BadRequestResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddOrder>> AddOrderAsync([FromBody] BriefOrder order)
    {
        if (order == null)
        {
            throw new NullReferenceException();
        }

        RepositoryOrder repositoryOrder = new RepositoryOrder(order.Id)
        {
            Customer = new RepositoryCustomer(new CustomerCode(order.CustomerId ?? throw new InvalidOperationException())),
            Employee = new RepositoryEmployee(order.EmployeeId),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new RepositoryShipper(order.ShipperId),
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = new RepositoryShippingAddress(order.ShipAddress, order.ShipCity, order.ShipRegion, order.ShipPostalCode, order.ShipCountry),
        };
        foreach (var orderDetail in order.OrderDetails)
        {
            repositoryOrder.OrderDetails.Add(new RepositoryOrderDetail(repositoryOrder)
            {
                Product = new RepositoryProduct(orderDetail.ProductId),
                UnitPrice = orderDetail.UnitPrice,
                Quantity = orderDetail.Quantity,
                Discount = orderDetail.Discount,
            });
        }

        try
        {
            var orderId = await this.orderRepository.AddOrderAsync(repositoryOrder);
            return new OkObjectResult(new AddOrder() { OrderId = orderId, });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(500);
        }
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        try
        {
            await this.orderRepository.RemoveOrderAsync(orderId);
            return new NoContentResult();
        }
        catch (OrderNotFoundException e)
        {
            Console.WriteLine(e);
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(500);
        }
    }

    [HttpPut("{orderId}")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, [FromBody] BriefOrder order)
    {
        try
        {
            await this.orderRepository.UpdateOrderAsync(new RepositoryOrder(orderId));
            return new NoContentResult();
        }
        catch (OrderNotFoundException e)
        {
            Console.WriteLine(e);
            return new NotFoundResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new StatusCodeResult(500);
        }
    }
}
