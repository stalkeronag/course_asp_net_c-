using Dotnet.Web.Interfaces;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Exceptions;
using FluentValidation;
using Dotnet.Web.Validation;

namespace Dotnet.Web.Controllers
{
    public class ProductsController : DotnetControllerBase
    {
    
        private readonly IProductService productService;

        private readonly ICommentService commentService;

        private readonly AbstractValidator<Product> validatorProduct;

        public ProductsController(ICommentService commentService, IProductService productService, AbstractValidator<Product> validatorProduct)
        {
            this.commentService = commentService;
            this.productService = productService;
            this.validatorProduct = validatorProduct;    
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        public async Task<IEnumerable<Product>> Get([FromQuery] PagingDto paging)
        {
           return await productService.GetListAsync(paging);
        }

        [HttpGet("{productId}/comments")]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), 200)]
        public async Task<IActionResult> GetComments([FromRoute] int productId)
        {
            if (productId > 1000)
            {
                return StatusCode(422);
            }

            return Ok(await commentService.GetComments(productId));
            
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }

            return Ok(await productService.GetProduct(id));
        }

    }
}
