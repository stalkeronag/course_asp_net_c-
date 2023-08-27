using Dotnet.Web.Admin.Data;
using Dotnet.Web.Admin.Dto;
using Dotnet.Web.Admin.Interfaces;
using Dotnet.Web.Admin.Models;

namespace Dotnet.Web.Admin.Services;

public class UserService : IUserService
{
    private readonly ApiContext apiContext;

    public UserService(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public async Task<UserDto> GetUser(int id)
    {
        User getUser = apiContext.Users.FirstOrDefault(u => u.Id == id);

        return MapUserToUserDto(getUser);
    }

    public Task<User> Login(LoginDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Register(RegisterDto registerDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Register(RegisterDto registerDto, Role role)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserDto>?> GetUserList()
    {
        var users = apiContext.Users;
        List<UserDto> userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            userDtos.Add(MapUserToUserDto(user));
        }

        return userDtos;
    }

    public async Task DeleteUser(int id)
    {
        User deleteUser = apiContext.Users.FirstOrDefault(u => u.Id == id);

        if (deleteUser != null)
        {
            apiContext.Users.Remove(deleteUser);
            await apiContext.SaveChangesAsync();
        }
    }

    private UserDto MapUserToUserDto(User user)
    {
        if (user == null)
        {
            return null;
        }

        return new UserDto()
        {
            Email = user.Email,
            UserName = user.UserName,
            UserId = user.Id,
        };
    }
}