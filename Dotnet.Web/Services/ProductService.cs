

using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Models;

namespace Dotnet.Web.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext context;

    public ProductService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddProduct(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }

    public Task AddProductToCart(int productId)
    {
        throw new NotImplementedException();
    }

    public Task EditProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetListAsync(PagingDto paging)
    {
        return Task.FromResult(context.Products.Skip(paging.Page).Take(paging.Take).AsEnumerable());
    }

    public Task<Product> GetProduct(long productId)
    {
        Product product = context.Products.Where(product => product.Id == productId).FirstOrDefault();
        return Task.FromResult(product);
    }

    public double GetRating(int id)
    {
       var comments =  context.Comments.Where(comment => comment.ProductId == id);
       List<CommentDto> listCommentDto = new List<CommentDto>();

       foreach (var comment in comments)
       {
            CommentDto commentDto = new CommentDto()
            {
                UserId = comment.UserId,
                CommentId = comment.Id,
                UserName = comment.User.UserName,
                ProductId = comment.ProductId,
                ProductName = comment.Product.Name,
                Text = comment.Text,
                Rating = comment.Rating
            };
            
            listCommentDto.Add(commentDto); 
       }

       int countComments = 0;
       int ratingSum = 0;
       double result;

       foreach (var comment in comments)
       {
            countComments++;
            ratingSum += comment.Rating;
       }

       return (double)ratingSum / (double)countComments;
    }
}