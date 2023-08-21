
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dotnet.Web.Data;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Web.Services;

public class JwtGeneratorService : IJwtGeneratorService
{
    private readonly AppDbContext context;

    private readonly IConfiguration configuration;

    private readonly UserManager<User> userManager;

    public JwtGeneratorService(AppDbContext context, UserManager<User> userManager, IConfiguration configuration)
    {
        this.context = context;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<string> CreateToken(User user)
    {
        int roleId = context.UserRoles.Where(role => role.UserId == user.Id).First().RoleId;
        string roleName = context.Roles.Where(role => role.Id == roleId).First().Name;

        var claims = new List<Claim> 
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)
        };
          
   
        var jwt = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}