using Dotnet.Web.Admin.Data;
using Dotnet.Web.Admin.Dto;
using Dotnet.Web.Admin.Exceptions;
using Dotnet.Web.Admin.Interfaces;
using Dotnet.Web.Admin.Models;
using Dotnet.Web.Admin.Services;
using Dotnet.Web.Admin.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UserFriendExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiContext>();
builder.Services.AddScoped<DbContext>
    (x => x.GetService<ApiContext>()!);

builder.Services.AddIdentity<User, UserRole>(config =>
    {
        config.Password.RequiredLength = 1;
        config.Password.RequireUppercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireDigit = false;
    })
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<ApiContext>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddTransient<AbstractValidator<RegisterDto>, RegisterDtoValidator>();

builder.Services.AddMvc();

builder.Services.AddRazorPages();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

Log.Logger = new LoggerConfiguration()  
    .Enrich.FromLogContext()
    .WriteTo.Seq("http://localhost:5002")
    .CreateLogger();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddMvc(option => option.EnableEndpointRouting = false).AddRazorPagesOptions(options => {
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });
builder.Services.AddAuthorization();

var app = builder.Build();
                                        
// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "test v1"));

app.UseCors(x => x.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();  
app.UseMvc();  
app.MapControllers();
app.UseStaticFiles();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => endpoints.MapRazorPages());
#pragma warning restore ASP0014
using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<ApiContext>();
DbSeeder.Seed(context,
    serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>(),
    serviceScope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>());
app.Run();