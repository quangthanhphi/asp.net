using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MenuBestSellerViewComponent: ViewComponent
{
    private readonly ApplicationDbContext _context;

    public MenuBestSellerViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await _context.Product.ToListAsync();
        return View(items);
    }
}
