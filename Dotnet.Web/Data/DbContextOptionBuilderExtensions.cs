using Microsoft.EntityFrameworkCore;

namespace Dotnet.Web.Data;

public static class DbContextOptionBuilderExtensions
{

    public static void ConfigureConnectionString(this DbContextOptionsBuilder optionsBuilder, IWebHostEnvironment env)
    {
        if (env.IsEnvironment("Test"))
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: $"Temp");
        }
    }
}