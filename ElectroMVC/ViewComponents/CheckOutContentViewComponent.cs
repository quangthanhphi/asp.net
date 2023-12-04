// MenuViewComponent.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CheckOutContentViewComponent: ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CheckOutContentViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var orderViewModel = new OrderViewModel(); 
        return View(orderViewModel);
    }


}
