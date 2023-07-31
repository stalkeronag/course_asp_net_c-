using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Dotnet.Web.Controllers
{
    public class CartController : DotnetControllerBase
    {
        private readonly AppDbContext context;
        public CartController(AppDbContext context) 
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(GetUserCartResponseDto), 200)]
        public IActionResult GetCart()
        {
           User user = AuntithicateUser();
           Cart cart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();

           if (cart == null)
           {
               cart =  CreateCart(user.Id);
           }

           var carts = context.CartProducts.Where(cartProduct => cartProduct.Id == cart.Id);

           GetUserCartResponseDto cartResponseDto = new GetUserCartResponseDto()
           {
             CartId = cart.Id,
             UserId = user.Id,
             Products = new List<ProductListDto>()
           };

           if (carts == null)
           {
                return Ok(cartResponseDto);
           }

           var productsInfo = carts.Select(cartProduct => new {cartProduct.Product, cartProduct.Count}).Join(
            context.Products,
            cartProduct => cartProduct.Product.Id,
            product => product.Id,
            (cartProduct, product) => new {Count = cartProduct.Count, ProductId = product.Id , Price = product.Price ,Name = product.Name}
           );

            List<ProductListDto> productListDtos = new List<ProductListDto>();

            foreach(var item in productsInfo)
            {
                ProductListDto productListDto = new ProductListDto()
                {
                    Price = item.Price,
                    ProductName = item.Name,
                    Count = item.Count,
                    ProductId = item.ProductId
                };
                productListDtos.Add(productListDto);
            }
          cartResponseDto.Products = productListDtos;

           return Ok(cartResponseDto);
        }

        [Authorize]
        [HttpDelete]
        public IActionResult CleanCart() 
        { 
           User user = AuntithicateUser();
           Cart cart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();

           if (cart == null)
           {
            return Ok();
           }
           else
           {
                var cartsProduct = context.CartProducts.Where(cartProduct => cartProduct.Cart.Id == cart.Id);
                
                if (cartsProduct == null)
                {
                    return Ok();
                }
                else
                {
                   context.CartProducts.RemoveRange(cartsProduct);
                   context.SaveChanges();
                   return Ok();
                }
           }
           
        }

        [Authorize]
        [HttpPut("{productId}")]
        public IActionResult UpdateCart([FromRoute] int productId)
        {
           Product product = context.Products.Where(product => product.Id == productId).First();
           User user = AuntithicateUser();
           Cart currentCart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();
           int generateCartProductId = 0;

           if (context.CartProducts.Count() != 0)
           {
            generateCartProductId = context.CartProducts.Select(cartProduct => cartProduct.Id).Max() + 1;
           }

           if (currentCart == null)
           {
             currentCart = CreateCart(user.Id);
             
             CartProduct cartProduct = new CartProduct()
             {
                Id = generateCartProductId,
                Count = 1,
                Product = product,
                Cart = currentCart
             };

             context.CartProducts.Add(cartProduct);
             context.SaveChanges();
             return Ok();
           }
           else
           {
               CartProduct cartProduct = context.CartProducts.Where(cartProduct => cartProduct.Product.Id == product.Id).FirstOrDefault();

               if (cartProduct == null)
               {

                 CartProduct addCartProduct = new CartProduct()
                 {
                    Id = generateCartProductId,
                    Count = 1,
                    Product = product,
                    Cart = currentCart
                 };

                 context.CartProducts.Add(addCartProduct);
                 context.SaveChanges();
               }
               else
               {
                cartProduct.Count += 1;
                context.SaveChanges();
               }
               return Ok();
           }      
        }

        private Cart CreateCart(int userId)
        {
            int generateCartId = 0;
            User user = context.Users.Where(user => user.Id == userId).First();
            if (context.Carts.Count() != 0)
            {
                generateCartId = context.Carts.Select(cart => cart.Id).Max() + 1;
            }

            Cart cart = new Cart()
            {
                Id = generateCartId,
                User = user,
                UserId = userId,
                Products = new List<CartProduct>()
            };

            context.Carts.Add(cart);
            context.SaveChanges();

            return cart;
        }

        private User AuntithicateUser()
        {
            string userName = User.Claims.First(claim => claim.Type.Equals("name")).Value;
            string email = User.Claims.First(claim => claim.Type.Equals("email")).Value;
            return context.Users.Where(user => user.UserName == userName && user.Email == email).First();
        }

    }
}
