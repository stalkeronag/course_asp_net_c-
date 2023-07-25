using Dotnet.Web.Data;
using Dotnet.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dotnet.Web.Controllers
{
    public class UsersController : DotnetControllerBase
    {
        private readonly AppDbContext context;

        public UsersController(AppDbContext context) 
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), 200)]
        public IActionResult GetUsers()
        {
            throw new NotImplementedException();
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        public IActionResult LoginUser([FromBody] LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(Boolean), 200)]
        public IActionResult RegisterUser([FromBody] RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("GetByEmail")]
        [ProducesResponseType(typeof(UserDto), 200)]
        public IActionResult GetByEmail([Required] string email)
        {
            throw new NotImplementedException();
        }

    }
}
