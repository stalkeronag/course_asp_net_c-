using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Dotnet.Web.Models;

public class UserRole : IdentityRole<int>
{
}