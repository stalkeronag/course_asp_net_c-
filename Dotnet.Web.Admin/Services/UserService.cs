using Dotnet.Web.Admin.Data;
using Dotnet.Web.Admin.Dto;
using Dotnet.Web.Admin.Interfaces;
using Dotnet.Web.Admin.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Dotnet.Web.Admin.Services;

public class UserService : IUserService
{
    private readonly ApiContext apiContext;

    private readonly UserManager<User> userManager;

    private readonly AbstractValidator<RegisterDto> registerDtovalidator;

    private const int roleAdminId = 2;

    private const int roleUserId = 1;

    public UserService(ApiContext apiContext, UserManager<User> userManager, AbstractValidator<RegisterDto> registerDtovalidator)
    {
        this.apiContext = apiContext;
        this.userManager = userManager;
        this.registerDtovalidator = registerDtovalidator;
    }

    public async Task<UserDto> GetUser(int id)
    {
        User getUser = apiContext.Users.FirstOrDefault(u => u.Id == id);

        return MapUserToUserDto(getUser);
    }

    public async Task<User> Login(LoginDto dto)
    {
        User loginUser = apiContext.Users.FirstOrDefault(user => user.Email == dto.Email);
        var users = apiContext.Users.ToArray();

        if (loginUser == null)
        {
            return null;
        }
        bool result = await userManager.CheckPasswordAsync(loginUser, dto.Password);

        if (!result)
        {
            return null;
        }
        var role = apiContext.Roles.Where(role => role.Name == "Admin").First();
        var userRole = apiContext.UserRoles.FirstOrDefault(userRole => userRole.UserId == loginUser.Id && userRole.RoleId == role.Id);

        if (userRole == null)
        {
            return null;
        }

        return loginUser;
    }

    public async Task<bool> Register(RegisterDto registerDto)
    {
        var resultValidation = registerDtovalidator.Validate(registerDto);

        if (!resultValidation.IsValid)
        {
            return false;
        }

        int idUser = 0;

        if (apiContext.Users.Count() > 0)
        {
            idUser = apiContext.Users.Select(u => u.Id).Max() + 1;
        }

        User user = new()
        {
            Id = idUser,
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        string hashPassword = userManager.PasswordHasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hashPassword;

        apiContext.Users.Add(user);
        await apiContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Register(RegisterDto registerDto, Role role)
    {
        bool resultRegistration = await Register(registerDto);

        if (!resultRegistration)
        {
            return false;
        }

        int idUser = apiContext.Users.Where(u => u.Email == registerDto.Email).Select(u => u.Id).FirstOrDefault();
        
        var userRole = apiContext.Roles.Where(role => role.Name == "User").FirstOrDefault();
        int idRole = userRole.Id;
        var adminRole = apiContext.Roles.Where(role => role.Name == "Admin").FirstOrDefault();
        if (role == Role.Admin)
        {
            idRole = adminRole.Id;
        }

        apiContext.UserRoles.Add(new IdentityUserRole<int>()
        {
            UserId = idUser,
            RoleId = idRole
        });
        await apiContext.SaveChangesAsync();

        return true;
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