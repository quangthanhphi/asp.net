// MenuViewComponent.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Partial_Item_CartViewComponent: ViewComponent
{
    private readonly ApplicationDbContext _context;

    public Partial_Item_CartViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart != null)
        {
            return View(cart.Items);
        }
        return View();
    }


}
