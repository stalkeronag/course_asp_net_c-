using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dotnet.Web.Controllers
{
    public class CommentController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public CommentController(AppDbContext context) 
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), 200)]
        public IActionResult GetComment(int id)
        {
          Comment comment = context.Comments.Where(comment => comment.Id == id).First();
          Product product = context.Products.Where(product => product.Id == comment.ProductId).First();
          User user = context.Users.Where(user => user.Id == comment.UserId).First();
          comment.User = user;
          comment.Product = product;

          if (comment == null)
          {
            return NotFound();
          }

          CommentDto commentDto = new CommentDto()
          {
            CommentId = comment.Id,
            UserId = comment.UserId,
            UserName = comment.User.UserName,
            ProductId = comment.ProductId,
            ProductName = comment.Product.Name,
            Text = comment.Text,
            Rating = comment.Rating
          };

          return Ok(commentDto);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult AddComment([FromBody] AddCommentDto addCommentDto)
        {
            string userName = User.Claims.First(claim => claim.Type.Equals("name")).Value;
            string email = User.Claims.First(claim => claim.Type.Equals("email")).Value;
            User user = context.Users.Where(user => user.UserName == userName && user.Email == email).First();
            Product productWithComment = context.Products.Where(product => product.Id == addCommentDto.ProductId).First();
            int commentId = 0;

            if (context.Comments != null)
            {
                commentId = context.Comments.Select(comment => comment.Id).Max() + 1;
            }
            Comment newComment = new Comment()
            {
                Product = productWithComment,
                ProductId = productWithComment.Id,
                User = user,
                UserId = user.Id,
                Text = addCommentDto.Text,
                Rating = addCommentDto.Rating,
                Id = commentId
            };
            
            context.Comments.Add(newComment);
            context.SaveChanges();
            return Ok(commentId);
        }

    }
}
