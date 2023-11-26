// MenuViewComponent.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MenuViewComponent: ViewComponent
{
    private readonly ApplicationDbContext _context;

    public MenuViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = await _context.Category.OrderBy(x => x.Position).ToListAsync();
        return View(items);
    }
}
