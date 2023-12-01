using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection; // Add this line for IServiceCollection
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Inside ConfigureServices method
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Inside Configure method
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "san-pham",
    pattern: "san-pham",
    defaults: new { controller = "Home", action = "AllProducts" });

app.MapControllerRoute(
    name: "thanh-toan",
    pattern: "thanh-toan",
    defaults: new { controller = "ShoppingCart", action = "CheckOut" });

app.MapControllerRoute(
    name: "gio-hang",
    pattern: "gio-hang",
    defaults: new { controller = "ShoppingCart", action = "Index" });

app.MapControllerRoute(
    name: "detailProduct",
    pattern: "chi-tiet/{id}",
    defaults: new { controller = "Product", action = "Details" });

app.MapControllerRoute(
    name: "danh-muc-san-pham",
    pattern: "danh-muc-san-pham/{id}",
    defaults: new { controller = "Home", action = "PCategory" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
