
using Dotnet.Web.Models;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;

namespace Dotnet.Web.Services;

public class OrderService : IOrderService
{
    private readonly IUserService userService;

    private readonly AppDbContext context;

    private int cachedOrderId;

    public OrderService(IUserService userService, AppDbContext context)
    {
        this.userService = userService;
        this.context = context;
        cachedOrderId = -1;
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
            await context.OrderProducts.AddAsync(orderProduct);

            order.Products.Add(orderProduct);
            order.Price += orderProduct.Product.Price * orderProduct.Count;
        }
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
        cachedOrderId = order.Id;

        return order.Id;  

    }

    public async Task<OrderDto> GetOrder()
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();

        Order order = context.Orders.Where(order => order.UserId == user.Id).FirstOrDefault();

        if (order == null)
        {
            return null;
        }

        List<ProductListDto> productListDtos = new List<ProductListDto>();

        var ordersProduct = context.OrderProducts.Where(orderProduct => orderProduct.Order.Id == order.Id);
        foreach (var orderProduct in ordersProduct)
        {
            Product product = context.Products.Where(product => product.Id == orderProduct.ProductId).FirstOrDefault();
            ProductListDto productListDto = new ProductListDto()
            {
                ProductName = product.Name,
                ProductId = product.Id,
                Price = orderProduct.Product.Price,
                Count = orderProduct.Count
            };

            productListDtos.Add(productListDto);
        }

        OrderDto orderDto = new OrderDto()
        {
            Price = order.Price,
            UserId = user.Id,
            OrderId = order.Id,
            UserName = user.UserName,
            OrderStatus = order.OrderStatus,
            Products  = productListDtos
        };

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
