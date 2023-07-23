namespace Dotnet.Intro.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            var app = builder.Build();

            app.UseRouting();

            app.MapGet("/", () => "Hello World!");

            app.MapGet("middleware/hello-world", () => "hello world!");

            app.MapControllers();

            app.Run();
        }

    }
}

