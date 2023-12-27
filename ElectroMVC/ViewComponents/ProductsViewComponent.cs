// ProductsViewComponent.cs

using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;
namespace ElectroMVC.ViewComponents
{
    public class ProductsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ProductsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? page)
        {
            var items = await _context.Product.ToListAsync();
            //var pageSize = 6;

            //var products = await _context.Product.Include(p => p.ProductCategory).ToPagedListAsync(page ?? 1, pageSize);

            return View(items);
        }
        
         
    }
    
}
