using Dotnet.Web.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Dotnet.Web.Admin.Pages
{
    
    public class GetUserModel : PageModel
    {
        
        private readonly IUserService userService;

        private readonly IHttpContextAccessor httpContextAccessor;

        public GetUserModel(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        
        public void OnGet()
        {
           
        }

        public IActionResult OnPost(int id)
        {
            
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Login");
            }

            return Page();
        }
    }
}
