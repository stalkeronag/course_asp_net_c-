using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace Dotnet.Web.Controllers
{
    public class OrderController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public OrderController(AppDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(OrderDto), 200)]
        public IActionResult GetOrder()
        {
            User user = AuntithicateUser();
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

            return Ok(orderDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult AddOrder()
        {
           User user = AuntithicateUser();
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
           context.SaveChanges();

           return Ok(order.Id);  
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Pay/{id}")]
        public IActionResult PayOrder([FromRoute] int id)
        {
            Order orderUser = context.Orders.Where(order => order.Id == id).First();

            orderUser.OrderStatus = OrderStatus.Payed;

            context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Ship/{id}")]
        public IActionResult ShipOrder([FromRoute] int id) 
        {
            Order orderUser = context.Orders.Where(order => order.Id == id).First();

            orderUser.OrderStatus = OrderStatus.Shipped;

            context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Dispute/{id}")]
        public IActionResult DisputeOrder([FromRoute] int id)
        {
            Order orderUser = context.Orders.Where(order => order.Id == id).First();

            orderUser.OrderStatus = OrderStatus.Disputed;

            context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Complete/{id}")]
        public IActionResult CompleteOrder([FromRoute] int id)
        {
            Order orderUser = context.Orders.Where(order => order.Id == id).First();

            orderUser.OrderStatus = OrderStatus.Completed;

            context.SaveChanges();

            return Ok();
        }

        private User AuntithicateUser()
        {
            string userName = User.Claims.First(claim => claim.Type.Equals("name")).Value;
            string email = User.Claims.First(claim => claim.Type.Equals("email")).Value;
            return context.Users.Where(user => user.UserName == userName && user.Email == email).First();
        }

    }
}
