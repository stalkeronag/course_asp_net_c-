using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class CartController : DotnetControllerBase
    {
        public CartController() 
        {
            
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
