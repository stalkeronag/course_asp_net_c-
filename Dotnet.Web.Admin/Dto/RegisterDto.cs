using System.ComponentModel.DataAnnotations;

namespace Dotnet.Web.Admin.Dto;

public class RegisterDto
{
    public required string Email { get; set; }
    
    public required string Password { get; set; }

    [Required]
    public required string UserName { get; set; } 
}