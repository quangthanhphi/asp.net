using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MenuProductCategoryViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public MenuProductCategoryViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await _context.ProductCategory.ToListAsync();
        return View(items);
    }
}
