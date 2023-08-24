using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Dotnet.Web.Interfaces;

namespace Dotnet.Web.Controllers
{
    public class CommentController : DotnetControllerBase
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService) 
        {
            this.commentService = commentService;
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), 200)]
        public async Task<CommentDto> GetComment(int id)
        {
          return await commentService.GetComment(id);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<int> AddComment([FromBody] AddCommentDto addCommentDto)
        {
            return await commentService.AddComment(addCommentDto);
        }

    }
}
