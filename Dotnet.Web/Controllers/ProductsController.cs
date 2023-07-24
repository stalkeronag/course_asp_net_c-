using Dotnet.Web.Dto;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class ProductsController : DotnetControllerBase
    {
        public ProductsController() { }

        [HttpGet]
        [ProducesResponseType(typeof(Product[]), 200)]
        public IActionResult GetProducts(int Take, int page)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{productId}/comments")]
        [ProducesResponseType(typeof(CommentDto), 200)]
        public IActionResult GetComments(int productId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        public IActionResult GetProduct(int id)
        {
            throw new NotImplementedException();
        }
    }
}
