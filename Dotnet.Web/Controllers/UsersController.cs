using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
namespace Dotnet.Web.Controllers
{
    public class UsersController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        private readonly IConfiguration configuration;

        private readonly UserManager<User> userManager;

        public UsersController(AppDbContext context, IConfiguration configuration, UserManager<User> userManager) 
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), 200)]
        public IActionResult GetCurrentUser()
        {
            string userName = User.Claims.First(claim => claim.Type.Equals("name")).Value;
            string email = User.Claims.First(claim => claim.Type.Equals("email")).Value;
            User user = context.Users.Where(user => user.UserName == userName && user.Email == email).First();

            UserDto userDto = new UserDto()
            {
                Email = email,
                UserId = user.Id,
                UserName = userName
            };
            return Ok(userDto);
        }

        
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            if (String.IsNullOrEmpty(loginDto.Email))
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(loginDto.Password))
            {
                return NotFound();
            }
            

            User user = context.Users.Where(user => user.Email.Equals(loginDto.Email)).First();

            if (user == null)
            {
                return NotFound();
            }

            int roleId = context.UserRoles.Where(role => role.UserId == user.Id).First().RoleId;
            UserRole userRole = context.Roles.Where(role => role.Id == roleId).First();
            PasswordVerificationResult passwordVerification = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            var claims = new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.Email, loginDto.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole.Name)
            };
          
            if (passwordVerification == PasswordVerificationResult.Failed)
            {
                return NotFound();
            }
   
            var jwt = new JwtSecurityToken(
                    issuer: configuration["Jwt:ValidIssuer"],
                    audience: configuration["Jwt:ValidAudience"],
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
            );

            
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                UserName = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwt)
            };

            return Ok(loginResponseDto);
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(Boolean), 200)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            if (String.IsNullOrEmpty(registerDto.Email))
            {
                return Ok(false);
            }

            if (String.IsNullOrEmpty(registerDto.Password))
            {
                return Ok(false);
            }

            if (String.IsNullOrEmpty(registerDto.UserName))
            {
                return Ok(false);
            }

            
            User user = new User()
            {
                Id = GenerateUserId(),
                Email =  registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            
            String hashPassword = userManager.PasswordHasher.HashPassword(user, registerDto.Password);
            user.PasswordHash = hashPassword;
            int userRoleId = context.Roles.Where(role => role.Name.Equals("User")).Select(role => role.Id).First();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            context.UserRoles.Add(new IdentityUserRole<int>(){
                RoleId = userRoleId,
                UserId = user.Id
            });
            await context.SaveChangesAsync();
            return Ok(true);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetByEmail")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public IActionResult GetByEmail([Required] string email)
        {
           User user = context.Users.Where(user => user.Email.Equals(email)).First();
           if (user == null)
           {
             return NotFound();
           }

           UserDto userDto = new UserDto()
           {
             Email = user.Email,
             UserName = user.UserName,
             UserId = user.Id
           };

           return Ok(userDto);
        }

        private int GenerateUserId()
        {
            return context.Users.Select(user => user.Id).Max() + 1;
        }
    }
}
