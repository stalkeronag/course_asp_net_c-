
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Models;

namespace Dotnet.Web.Services;

public class OrderService : IOrderService
{
    private readonly IUserService userService;

    private readonly AppDbContext context;

    public OrderService(IUserService userService, AppDbContext context)
    {
        this.userService = userService;
        this.context = context;
    }

    public async Task<int> CreateOrder()
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Cart cartUser = context.Carts.Where(cart => cart.UserId == user.Id).First();
        int generateIdOrder = 0;
           
        if (context.Orders.Count() != 0)
        {
            generateIdOrder = context.Orders.Select(order => order.Id).Max() + 1;
        }
        Order order = new Order()
        {
            Price = 0,
            User = user,
            UserId = user.Id,
            Id = generateIdOrder,
            OrderStatus = OrderStatus.New,
            Products = new List<OrderProduct>()
        };
        var cartsProducts = context.CartProducts.Where(cartProduct => cartProduct.Cart.Id == cartUser.Id);
        var productsInfo = cartsProducts.Select(cartProduct => new {cartProduct.Product, cartProduct.Count}).Join(
        context.Products,
        cartProduct => cartProduct.Product.Id,
        product => product.Id,
        (cartProduct, product) => new {Count = cartProduct.Count, ProductId = product.Id , Price = product.Price ,Name = product.Name}
        );

        int generateIdOrderProduct = 0;

        if (context.OrderProducts.Count() != 0)
        {
            generateIdOrderProduct = context.OrderProducts.Select(orderProduct => orderProduct.Id).Max() + 1;
        }

        foreach (var item in productsInfo)
        {

            OrderProduct orderProduct = new OrderProduct()
            {
                Id = generateIdOrderProduct,
                Count = item.Count,
                Product = context.Products.Where(product => product.Id == item.ProductId).First(),
                ProductId = item.ProductId,
                Order = order
            };
                
            generateIdOrderProduct++;
            context.OrderProducts.Add(orderProduct);

            order.Products.Add(orderProduct);
            order.Price += orderProduct.Product.Price * orderProduct.Count;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return order.Id;  

    }

    public async Task<OrderDto> GetOrder()
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Cart cartUser = context.Carts.Where(cart => cart.UserId == user.Id).First();
        Order orderUser = context.Orders.Where(order => order.UserId == user.Id).First();

        var cartsProducts = context.CartProducts.Where(cartProduct => cartProduct.Cart.Id == cartUser.Id);
        var productsInfo = cartsProducts.Select(cartProduct => new {cartProduct.Product, cartProduct.Count}).Join(
        context.Products,
        cartProduct => cartProduct.Product.Id,
        product => product.Id,
        (cartProduct, product) => new {Count = cartProduct.Count, ProductId = product.Id , Price = product.Price ,Name = product.Name}
        );

        OrderDto orderDto = new OrderDto()
        {
            OrderId = orderUser.Id,
            UserId = user.Id,
            UserName = user.UserName,
            Price = orderUser.Price,
            OrderStatus = orderUser.OrderStatus,
            Products = new List<ProductListDto>()
        };

        foreach (var item in productsInfo)
        {
            ProductListDto productListDto = new ProductListDto()
            {
                ProductId = item.ProductId,
                ProductName = item.Name,
                Price = item.Price,
                Count = item.Count
            };

            orderDto.Products.Add(productListDto);
        }

        return orderDto;

    }

    public async Task MoveOrderStatus(int orderId, OrderStatus orderStatus)
    {
        Order order = context.Orders.Where(order => order.Id == orderId).FirstOrDefault();

        if (order != null)
        {
            order.OrderStatus = orderStatus;
        }

        await context.SaveChangesAsync();
    }
}
