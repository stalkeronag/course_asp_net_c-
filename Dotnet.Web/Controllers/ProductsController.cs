using Dotnet.Web.Interfaces;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class ProductsController : DotnetControllerBase
    {
    
        private readonly IProductService productService;

        private readonly ICommentService commentService;

        public ProductsController(ICommentService commentService, IProductService productService)
        {
            this.commentService = commentService;
            this.productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Product[]), 200)]
        public async Task<Product[]> GetProducts(int Take, int page)
        {
            PagingDto pagingDto = new PagingDto()
            {
                Take = Take,
                Page = page
            };
            
            return (await productService.GetListAsync(pagingDto)).ToArray();
        }

        [HttpGet("{productId}/comments")]
        [ProducesResponseType(typeof(CommentDto[]), 200)]
        public async Task<CommentDto[]> GetComments(int productId)
        {
            CommentDto[] commentDtos = (await commentService.GetComments(productId)).ToArray();
            return commentDtos;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        public async Task<Product> GetProduct(int id)
        {
            return await productService.GetProduct(id);
        }
    }
}
