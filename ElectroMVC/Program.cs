using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ElectroMVC.Data;
using ElectroMVC.Models;
using System;
using ElectroMVC.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var preserveLoginUrl = builder.Configuration.GetValue<bool>("PreserveLoginUrl");
var loginUrl = builder.Configuration["Authentication:Forms:LoginUrl"];
var timeout = builder.Configuration.GetValue<int>("Authentication:Forms:Timeout");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

//builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
//{
//    { "autoFormsAuthentication", "false" },
//    { "enableSimpleMembership", "false" }
//});

// Inside ConfigureServices method
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpContextAccessor();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.AddScoped<ApplicationUserManager>();
builder.Services.AddScoped<ApplicationSignInManager>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    //options.AccessDeniedPath = "/Account/AccessDenied";
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
//});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.AllowedForNewUsers = false; // Cho phép không khóa tài khoản mặc định
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Inside Configure method
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "chi-tiet-tin-tuc",
    pattern: "chi-tiet-tin-tuc/{id}",
    defaults: new { controller = "News", action = "Info" });

app.MapControllerRoute(
    name: "detailProduct",
    pattern: "chi-tiet/{id}",
    defaults: new { controller = "Product", action = "Details" });

app.MapControllerRoute(
    name: "gioi-thieu",
    pattern: "gioi-thieu",
    defaults: new { controller = "Home", action = "Contact" });


app.MapControllerRoute(
    name: "san-pham",
    pattern: "san-pham",
    defaults: new { controller = "Home", action = "AllProducts" });

app.MapControllerRoute(
    name: "san-pham-2",
    pattern: "/ShoppingCart/san-pham",
    defaults: new { controller = "Home", action = "AllProducts" });

app.MapControllerRoute(
    name: "thanh-toan",
    pattern: "thanh-toan",
    defaults: new { controller = "ShoppingCart", action = "CheckOut" });

app.MapControllerRoute(
    name: "CheckOut1",
    pattern: "CheckOut1",
    defaults: new { controller = "ShoppingCart", action = "CheckOut1" });

app.MapControllerRoute(
    name: "ThanhToan",
    pattern: "vnpay_return",
    defaults: new { controller = "ShoppingCart", action = "VNpayReturn" });

app.MapControllerRoute(
    name: "gio-hang",
    pattern: "gio-hang",
    defaults: new { controller = "ShoppingCart", action = "Index" });



app.MapControllerRoute(
    name: "tin-tuc",
    pattern: "tin-tuc",
    defaults: new { controller = "News", action = "Details" });



app.MapControllerRoute(
    name: "danh-muc-san-pham",
    pattern: "danh-muc-san-pham/{id}",
    defaults: new { controller = "Home", action = "PCategory" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
