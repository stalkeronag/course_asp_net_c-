using System.Security.Claims;
using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Dotnet.Web.Services;

public class UserService : IUserService
{
    private readonly AppDbContext context;

    private readonly UserManager<User> userManager;

    private readonly IJwtGeneratorService jwtGeneratorService;

    private readonly IHttpContextAccessor httpContextAccessor;

    public UserService(AppDbContext context, UserManager<User> userManager, IJwtGeneratorService jwtGeneratorService, IHttpContextAccessor httpContextAccessor)
    {
        this.context = context;
        this.userManager = userManager;
        this.jwtGeneratorService = jwtGeneratorService;
        this.httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    public async Task<UserDto> GetUser()
    {
        HttpContext httpContext= httpContextAccessor.HttpContext;
        string userName = httpContext.User.Claims.First(claim => claim.Type.Equals("name")).Value;
        string email = httpContext.User.Claims.First(claim => claim.Type.Equals("email")).Value;
        User user = context.Users.Where(user => user.UserName == userName && user.Email == email).First();

        UserDto userDto = new UserDto()
        {
            Email = email,
            UserId = user.Id,
            UserName = userName
        };

        return userDto;
    }

    public async Task<UserDto> GetUserByEmail(string email)
    {
        User user = context.Users.Where(user => user.Email == email).FirstOrDefault();

        if (user == null)
            return null;
        
        UserDto userDto = new UserDto()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = email
        };

        return userDto;
    }

    public async Task<LoginResponseDto> Login(LoginDto dto)
    {
        User user = context.Users.Where(user => user.Email == dto.Email).FirstOrDefault();

        if (user == null)
            return null;
        
        string token = await jwtGeneratorService.CreateToken(user);

        LoginResponseDto loginResponseDto = new LoginResponseDto()
        {
            UserName = user.UserName,
            Token = token
        };

        return loginResponseDto;
    }

    public async Task<bool> Register(RegisterDto registerDto)
    {
        if (string.IsNullOrEmpty(registerDto.Email))
            return false;

        if (string.IsNullOrEmpty(registerDto.Password))
            return false;
        
        if (string.IsNullOrEmpty(registerDto.UserName))
            return false;

        User user = new()
        {
            Id = GenerateUserId(),
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        string hashPassword = userManager.PasswordHasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hashPassword;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        int userRoleId = context.Roles.Where(role => role.Name.Equals("User")).Select(role => role.Id).First();
        context.UserRoles.Add(new IdentityUserRole<int>(){
            RoleId = userRoleId,
            UserId = user.Id
        });

        await context.SaveChangesAsync();
        return true;
    }

    private int GenerateUserId()
    {
        return context.Users.Select(user => user.Id).Max() + 1;
    }
}