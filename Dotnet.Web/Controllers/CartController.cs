using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Dotnet.Web.Interfaces;

namespace Dotnet.Web.Controllers
{
    public class CartController : DotnetControllerBase
    {

        private readonly ICartService cartService;
        public CartController(ICartService cartService) 
        {
            this.cartService = cartService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(GetUserCartResponseDto), 200)]
        public async Task<GetUserCartResponseDto> GetCart()
        {
           return await cartService.GetUserCart();
        }

        [Authorize]
        [HttpDelete]
        public async void CleanCart() 
        { 
          await cartService.CleanCart();
        }

        [Authorize]
        [HttpPut("{productId}")]
        public async void UpdateCart([FromRoute] int productId)
        {
           await cartService.UpdateCart(productId);
        }

      
    }
}
