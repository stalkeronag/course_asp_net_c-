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
using Dotnet.Web.Interfaces;
using Dotnet.Web.Admin.Services;

namespace Dotnet.Web.Controllers
{
    public class UsersController : DotnetControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) 
        {
            this.userService = userService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), 200)]
        public  async Task<UserDto> GetCurrentUser()
        {
            return await userService.GetUser();
        }

        
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        public async Task<LoginResponseDto> LoginUser([FromBody] LoginDto loginDto)
        {
            return await userService.Login(loginDto);
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(Boolean), 200)]
        public IActionResult RegisterUser([FromBody] RegisterDto registerDto)
        {
           return Ok(userService.Register(registerDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetByEmail")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public IActionResult GetByEmail([Required] string email)
        {
           return Ok(userService.GetUserByEmail(email));
        }

    }
}
