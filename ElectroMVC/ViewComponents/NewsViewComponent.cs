// ProductsViewComponent.cs

using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectroMVC.ViewComponents
{
    public class NewsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NewsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _context.News.ToListAsync();
            return View(items);  
        }
        
         
    }
}
