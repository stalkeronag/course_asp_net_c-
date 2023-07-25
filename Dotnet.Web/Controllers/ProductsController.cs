using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class ProductsController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public ProductsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Product[]), 200)]
        public IActionResult GetProducts(int Take, int page)
        {
            return Ok(context.Products.Skip(page).Take(Take).ToArray());
        }

        [HttpGet("{productId}/comments")]
        [ProducesResponseType(typeof(CommentDto[]), 200)]
        public IActionResult GetComments(int productId)
        {
            Product product = context.Products.Where(product =>  product.Id == productId).FirstOrDefault();
            Comment[] comments = context.Comments.Where(comment => comment.ProductId == productId).ToArray();
            CommentDto[] commentsDto = new CommentDto[comments.Length];
            for (int i = 0; i < comments.Length; i++)
            {
                commentsDto[i] = new CommentDto()
                {
                    CommentId = comments[i].Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UserName = comments[i].User?.UserName,
                    UserId = comments[i].UserId,
                    Text = comments[i].Text,
                    Rating = comments[i].Rating
                };
            }
            return Ok(commentsDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        public IActionResult GetProduct(int id)
        {
            Product product = context.Products.Where(product =>  product.Id == id).FirstOrDefault();
            return Ok(product);
        }
    }
}
