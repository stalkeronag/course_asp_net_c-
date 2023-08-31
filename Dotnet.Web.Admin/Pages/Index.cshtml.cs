using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dotnet.Web.Admin.Pages;

public class Index : PageModel
{
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync(string path)
    {
        return Redirect("/" + path);
    }
}