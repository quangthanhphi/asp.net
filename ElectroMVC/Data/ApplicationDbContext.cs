using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace ElectroMVC.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> manager)
        {
            var userIdentity = new ClaimsIdentity(await manager.GetClaimsAsync(this), "Identity.Application");
            return userIdentity;
        }
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ElectroMVC.Models.User> User { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Category> Category { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Adv> Adv { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Contact> Contact { get; set; } = default!;
        public DbSet<ElectroMVC.Models.News> News { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Order> Order { get; set; } = default!;
        public DbSet<ElectroMVC.Models.OrderDetail> OrderDetail { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Posts> Posts { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Product> Product { get; set; } = default!;
        public DbSet<ElectroMVC.Models.ProductCategory> ProductCategory { get; set; } = default!;
        public DbSet<ElectroMVC.Models.Subscribe> Subscribe { get; set; } = default!;
        public DbSet<ElectroMVC.Models.SystemSetting> SystemSetting { get; set; } = default!;
    }
}
