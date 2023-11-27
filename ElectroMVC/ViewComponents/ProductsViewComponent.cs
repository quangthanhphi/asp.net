// ProductsViewComponent.cs

using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectroMVC.ViewComponents
{
    public class ProductsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ProductsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.Product.ToListAsync();
            return View(items);  // Assuming you have a view named Default.cshtml
        }
        
    }
}
