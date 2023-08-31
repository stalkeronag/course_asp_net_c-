using Dotnet.Web.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace Dotnet.Web.Admin.Data
{
    public class DbSeeder
    {
        public static async void Seed(ApiContext apiContext, UserManager<User> userManager, RoleManager<UserRole> roleManager)
        {
            string[] roles = new string[] { "Admin", "User" };

            foreach (string role in roles)
            {
                var ifExistRole = await roleManager.RoleExistsAsync(role);
                if (!ifExistRole)
                {
                    await roleManager.CreateAsync(new UserRole()
                    {
                        Name = role
                    });
                }
            }

            await apiContext.SaveChangesAsync();

            string adminEmail = "someTest@gmail.com";
            var user = new User
            {
                Id = 2,
                Email = adminEmail,
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "ADMIN",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            userManager.CreateAsync(user, "secret");
            await apiContext.SaveChangesAsync();

            var newUser = await userManager.FindByEmailAsync(adminEmail);
            await userManager.AddToRoleAsync(newUser!, "Admin");
            await apiContext.SaveChangesAsync();
        }
    }
}
