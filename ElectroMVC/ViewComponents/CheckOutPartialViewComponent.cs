// MenuViewComponent.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CheckOutPartialViewComponent: ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CheckOutPartialViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart != null)
        {
            ViewBag.CheckCart = cart;
            return View(cart.Items);
        }
        return View();
    }
        

}
