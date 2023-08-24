using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Dotnet.Web.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Dotnet.Web.Admin.Exceptions;

namespace Dotnet.Web.Controllers
{
    public class CommentController : DotnetControllerBase
    {
        private readonly ICommentService commentService;

        private readonly AbstractValidator<AddCommentDto> validatorAddCommentDto;

        private readonly AbstractValidator<Product> validatorProduct;
        public CommentController(ICommentService commentService, AbstractValidator<AddCommentDto> validatorAddCommentDto, AbstractValidator<Product> validatorProduct) 
        {
            this.commentService = commentService;
            this.validatorAddCommentDto = validatorAddCommentDto;
            this.validatorProduct = validatorProduct;
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), 200)]
        public async Task<IActionResult> GetComment(int id)
        {
            if (id > 1000)
            {
                return StatusCode(422);
            }

            return Ok(await commentService.GetComment(id));    
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto addCommentDto)
        {
            var resultValidationAddCommentDto = validatorAddCommentDto.Validate(addCommentDto);

            if (!resultValidationAddCommentDto.IsValid)
            {
                return StatusCode(422);
            }

            if (addCommentDto.ProductId > 1000)
            {
                return StatusCode(422);
            }

            return Ok(await commentService.AddComment(addCommentDto));
        }


    }
}
