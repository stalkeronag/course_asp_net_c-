using System.Reflection;
using Dotnet.Web.Attributes;
using Dotnet.Web.Controllers;
using Dotnet.Web.Data;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

static void ConfigureAuth(WebApplicationBuilder builder)
{
}

static void ConfigureValidators(WebApplicationBuilder builder)
{
}

static void ConfigureApi(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(options => 
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "����� ���������� �� ���� �� ASP.NET Core!",
                    Description = "<p>���� ���� ������������ ��� ���, ��� ����� ������� ���������� ���-���������� � �������������� ����������� ���������� ASP.NET Core.</p><p>�� �������, ��� ������������ ASP.NET Core ��� �������� ���-����������, ������� � ��������� � ��������� ������� � ���������� �������������� ���������� �� ��������.�� ����� ������������� � ��������� ����������� ������������������� �������� � ������ ������, ���������������, ������������ � ������ ������.</p><p>���� ������� ��� �������� ���� ������ � ��������� �������� � ������� ���-����������.��� ��� ������� ������ � ���������, ��� ����� ���������� ASP.NET Core ��� ������ ���������� �������!</p>",
                    Version = "v1"
                }
        ));
}

static void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services
        .AddIdentity<User, UserRole>()
        .AddEntityFrameworkStores<AppDbContext>();   
}

static void ConfigureDb(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<DbContext>(x => x.GetRequiredService<AppDbContext>());
}

static void ConfigureLogger(WebApplicationBuilder builder)
{
}

static void InitDb(WebApplication app)
{
    if(bool.TryParse(app.Configuration["Database:SkipInitialization"], out var skip))
    {
        if (skip)
        {
            return;
        }
    }
    
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.IsRelational())
    {
        context.Database.Migrate();
    }

    DbSeeder.Seed(context, 
        serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>(),
        serviceScope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>());
}

static void RunApp(WebApplicationBuilder builder)
{
    var app = builder.Build();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "version 1"));

    app.UseCors(x => x.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );

    app.UseAuthorization();
    app.MapControllers();

    var hw = typeof(DotnetControllerBase)
        .Assembly
        .GetCustomAttribute<HomeworkProgressAttribute>()?.Number;
    if (hw > 4)
    {
        InitDb(app);
    }
   

    app.Run();
}

var b = WebApplication.CreateBuilder(args);

ConfigureApi(b);
ConfigureDb(b);
ConfigureIdentity(b);
ConfigureServices(b);
ConfigureValidators(b);
ConfigureLogger(b);
ConfigureAuth(b);
RunApp(b);