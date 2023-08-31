using Dotnet.Web.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dotnet.Web.Admin.Pages
{
    public class RegisterPageModel : PageModel
    {
        private readonly IUserService userService;

        public RegisterPageModel(IUserService userService)
        {
            this.userService = userService;
        }
        
        public void OnGet()
        {

        }

        public async void OnPost(string password, string email, string userName)
        {

        }
    }
}
