using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class CartController : DotnetControllerBase
    {
        private readonly AppDbContext context;
        public CartController(AppDbContext context) 
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetUserCartResponseDto), 200)]
        public IActionResult GetCart()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public IActionResult DeleteCart() 
        { 
            throw new NotImplementedException(); 
        }

        [HttpPut("{productId}")]
        public IActionResult AppendCart(int id)
        {
            throw new NotImplementedException();
        }

    }
}
