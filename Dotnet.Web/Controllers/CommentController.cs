using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Web.Controllers
{
    public class CommentController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public CommentController(AppDbContext context) 
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), 200)]
        public IActionResult GetComment(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult AddComment([FromBody] AddCommentDto addCommentDto)
        {
            throw new NotImplementedException();
        }

    }
}
