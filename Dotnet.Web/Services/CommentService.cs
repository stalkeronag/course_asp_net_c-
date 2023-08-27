

using Dotnet.Web.Models;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;
using FluentValidation.Validators;
using FluentValidation;
using Dotnet.Web.Admin.Exceptions;
using System.Data;

public class CommentService : ICommentService
{
    private readonly AppDbContext context;

    private readonly IUserService userService;

    public CommentService(AppDbContext context, IUserService userService)
    {
        this.context = context;
        this.userService = userService;
    }

    public async Task<int> AddComment(AddCommentDto comment)
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Product product = context.Products.Where(product => product.Id == comment.ProductId).FirstOrDefault();

        
        Comment addComment = new Comment()
        {
            Id = context.Comments.Select(comm => comm.Id).Max() + 1,
            User = user,
            UserId = user.Id,
            Product = product,
            ProductId = comment.ProductId,
            Text = comment.Text,
            Rating = comment.Rating
        };

        context.Comments.Add(addComment);
        await context.SaveChangesAsync();

        return addComment.Id;
    }

    public Task<CommentDto> GetComment(int id)
    {
        Comment comment = context.Comments.Where(comment => comment.Id == id).FirstOrDefault();
        Product product= context.Products.Where(product => product.Id == comment.ProductId).FirstOrDefault();

        CommentDto commentDto = new CommentDto()
        {
            UserId = comment.UserId,
            CommentId = comment.Id,
            UserName = comment.User?.UserName,
            ProductId = comment.ProductId,
            ProductName = product?.Name,
            Text = comment.Text,
            Rating = comment.Rating
        };

        return Task.FromResult(commentDto);
    }

    public async Task<IEnumerable<CommentDto>> GetComments(int productId)
    {
       var comments =  context.Comments.Where(comment => comment.ProductId == productId);
       List<CommentDto> listCommentDto = new List<CommentDto>();
       Product product= context.Products.Where(product => product.Id == productId).FirstOrDefault();

       

       foreach (var comment in comments)
       {
            CommentDto commentDto = new CommentDto()
            {
                UserId = comment.UserId,
                CommentId = comment.Id,
                UserName = comment.User?.UserName,
                ProductId = comment.ProductId,
                ProductName = product?.Name,
                Text = comment.Text,
                Rating = comment.Rating
            };
            
            listCommentDto.Add(commentDto); 
       }

       return listCommentDto.AsEnumerable();
    }

 
}