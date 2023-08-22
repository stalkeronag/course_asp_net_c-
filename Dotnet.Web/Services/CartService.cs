
using Dotnet.Web.Models;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;


namespace Dotnet.Web.Services;

public class CartService : ICartService
{
    private readonly IUserService userService;

    private readonly AppDbContext context;

    private readonly IOrderService orderService;

    public CartService(IUserService userService, AppDbContext context)
    {
        this.userService = userService;
        this.context = context;
    }
    
    public async Task CleanCart()
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Cart currentCart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();

        if (currentCart == null)
        {
            return;
        }
        else
        {
            var cartProducts = context.CartProducts.Where(cartProduct => cartProduct.Cart.Id == currentCart.Id);

            if (cartProducts == null)
            {
                return;
            }
            else
            {
                context.CartProducts.RemoveRange(cartProducts);
                context.SaveChanges();
            }
        }
    }

    public async Task<GetUserCartResponseDto> GetUserCart()
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Cart currentCart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();

        GetUserCartResponseDto getUserCartResponseDto = new GetUserCartResponseDto()
        {
            UserId = user.Id,
            Products = new List<ProductListDto>()
        };

        if (currentCart == null)
        {
            currentCart = await CreateCart(user);
            getUserCartResponseDto.CartId = currentCart.Id;
            return getUserCartResponseDto;
        }

        getUserCartResponseDto.CartId = currentCart.Id;
        var cartsProducts = context.CartProducts.Where(cartProduct => cartProduct.Cart.Id == currentCart.Id);
        
        if (cartsProducts == null)
        {
            return getUserCartResponseDto;
        }

        var productsInfo = cartsProducts.Join(context.Products,
                            cartProduct => cartProduct.Product.Id,
                            product => product.Id,
                            (cp, p) => new {Count = cp.Count, ProductName = p.Name, ProductId = p.Id, Price = p.Price});

        foreach (var productInfo in productsInfo)
        {
            ProductListDto addProductListDto= new ProductListDto()
            {
                ProductName = productInfo.ProductName,
                ProductId = productInfo.ProductId,
                Price = productInfo.Price,
                Count = productInfo.Count
            };

            getUserCartResponseDto.Products.Add(addProductListDto);
        }

       
        return getUserCartResponseDto;

    }

    public async Task UpdateCart(int productId)
    {
        UserDto userDto = await userService.GetUser();
        User user = context.Users.Where(user => user.Id == userDto.UserId).FirstOrDefault();
        Product product = context.Products.Where(product => product.Id == productId).FirstOrDefault();
        Cart currentCart = context.Carts.Where(cart => cart.UserId == user.Id).FirstOrDefault();

        int idCartProduct = 0;

        if (context.CartProducts.Count() != 0)
        {
            idCartProduct = context.CartProducts.Select(cartProduct => cartProduct.Id).Max() + 1;
        }

        if (currentCart == null)
        {
            currentCart = await CreateCart(user);

            CartProduct firstCartProduct = new CartProduct()
            {
                Cart = currentCart,
                Id = idCartProduct,
                Product = product,
                Count = 1
            };

            await context.CartProducts.AddAsync(firstCartProduct);
            await context.SaveChangesAsync();
            return;
        }
        else
        {
            CartProduct currentCartProduct = context.CartProducts.Where(cartProduct => cartProduct.Product.Id == productId)
                .Where(cartProduct => cartProduct.Cart.Id == currentCart.Id).FirstOrDefault();

            if (currentCartProduct == null)
            {
                currentCartProduct = new CartProduct()
                {
                    Cart = currentCart,
                    Count = 1,
                    Product = product,
                    Id = idCartProduct
                };

                await context.CartProducts.AddAsync(currentCartProduct);
                await context.SaveChangesAsync();
            }
            else
            {
                currentCartProduct.Count++;
                await context.SaveChangesAsync();
            }
        }
    }

    private async Task<Cart> CreateCart(User user)
    {
        List<CartProduct> cartProducts = new List<CartProduct>();
        int cartId = 0;
        if (context.Carts.Count() > 0)
        {
           cartId = context.Carts.Select(cart => cart.Id).Max() + 1;
        }
        
        Cart newCart = new Cart()
        {
            Id = cartId,
            User = user,
            UserId = user.Id,
            Products = cartProducts
        };
        await context.Carts.AddAsync(newCart);

        await context.SaveChangesAsync();

        return newCart;
    }
}

    

