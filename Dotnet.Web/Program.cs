using System.Reflection;
using Dotnet.Web.Attributes;
using Dotnet.Web.Controllers;
using Dotnet.Web.Data;
using Dotnet.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Dotnet.Web.Interfaces;
using Dotnet.Web.Services;
using Microsoft.AspNetCore.Components.RenderTree;

static void ConfigureAuth(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => 
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
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
    {
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Добро пожаловать на курс по ASP.NET Core!",
                    Description = "<p>Этот курс предназначен для тех, кто хочет освоить разработку веб-приложений с использованием популярного фреймворка ASP.NET Core.</p><p>Вы узнаете, как использовать ASP.NET Core для создания веб-приложений, начиная с установки и настройки проекта и заканчивая развертыванием приложения на хостинге.Вы также познакомитесь с основными концепциями фреймворканаучитесь работать с базами данных, аутентификацией, авторизацией и многим другим.</p><p>Курс поможет вам улучшить свои навыки и расширить кругозор в области веб-разработки.Так что давайте начнем и посмотрим, что может предложить ASP.NET Core для вашего следующего проекта!</p>",
                    Version = "v1"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        new string[] {}
                    }
                });
    });
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<IJwtGeneratorService, JwtGeneratorService>();
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<ICommentService, CommentService>();
    builder.Services.AddTransient<IProductService, ProductService>();
    builder.Services.AddTransient<ICartService, CartService>();
    builder.Services.AddTransient<IOrderService, OrderService>();
}

static void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services
        .AddIdentity<User, UserRole>()
        .AddEntityFrameworkStores<AppDbContext>();   
}

static void ConfigureDb(WebApplicationBuilder builder)
{
    var env = builder.Environment;
    if ( env.IsDevelopment())
    {
        string connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Default");
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });
    }
    else if (env.IsEnvironment("Test"))
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureConnectionString(env);
        });
    }
    else if (env.IsProduction())
    {
        string connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Default");
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
        });
    }
    builder.Services.AddScoped<DbContext>(x => x.GetRequiredService<AppDbContext>());
}

static void ConfigureLogger(WebApplicationBuilder builder)
{
}

static void InitDb(WebApplication app)
{
    if (bool.TryParse(app.Configuration["Database:SkipInitialization"], out var skip))
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
    app.UseAuthentication();
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