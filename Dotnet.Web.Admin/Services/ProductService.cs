using Dotnet.Web.Admin.Data;
using Dotnet.Web.Admin.Interfaces;
using Dotnet.Web.Admin.Models;

namespace Dotnet.Web.Admin.Services;

public class ProductService : IProductService
{
    private readonly ApiContext apiContext;

    public ProductService(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public async Task<List<Product>?> GetProductList()
    {
        return apiContext.Products.ToList();
    }

    public async Task AddProduct(Product product)
    {
        if (product == null)
        {
            return;
        }

        if (!apiContext.Products.Select(product => product.Id).Contains(product.Id))
        {
            apiContext.Products.Add(product);
            await apiContext.SaveChangesAsync();
        }
    }

    public async Task DeleteProduct(int id)
    {
        Product deleteProduct = apiContext.Products.FirstOrDefault(p => p.Id == id);

        if (deleteProduct != null)
        {
            apiContext.Products.Remove(deleteProduct);
            await apiContext.SaveChangesAsync();
        }
    }
}