using Dotnet.Web.Data;
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
            return Ok(DbSeeder.Products.ToArray());
        }

        [HttpGet("{productId}/comments")]
        [ProducesResponseType(typeof(CommentDto[]), 200)]
        public IActionResult GetComments(int productId)
        {
            Product product = DbSeeder.Products.Where(product =>  product.Id == productId).FirstOrDefault();
            Comment comment = DbSeeder.Comment;
            CommentDto commentDto = new CommentDto()
            {
                CommentId = comment.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                UserName = comment.User.UserName,
                UserId = comment.UserId,
                Text = comment.Text,
                Rating = comment.Rating
            };
            CommentDto[] commentsDto = new CommentDto[] { commentDto };
            return Ok(commentsDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        public IActionResult GetProduct(int id)
        {
            Product product = DbSeeder.Products.Where(product =>  product.Id == id).FirstOrDefault();
            return Ok(product);
        }
    }
}
