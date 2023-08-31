using Dotnet.Web.Admin.Dto;
using Dotnet.Web.Admin.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dotnet.Web.Admin.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService userService;

        private readonly IHttpContextAccessor httpContextAccessor;

        public LoginModel(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult>  OnPostAsync(string email, string password)
        {

            LoginDto loginDto = new LoginDto()
            { 
                Email = email, 
                Password = password 
            };


            Models.User user = await userService.Login(loginDto);

            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect("/Index");
         
        }
    }
}
