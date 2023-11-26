// MenuArrivalPartialViewComponent.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MenuArrivalPartialViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public MenuArrivalPartialViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    // MenuArrivalPartialViewComponent.cs
    public async Task<IViewComponentResult> InvokeAsync(int productCategoryId)
    {
        var products = await _context.Product
            .Where(p => p.ProductCategoryId == productCategoryId)
            .ToListAsync();

        var productViewModels = new List<ProductViewModel>();

        foreach (var product in products)
        {
            var productCategory = await _context.ProductCategory
                .Where(pc => pc.Id == product.ProductCategoryId)
                .FirstOrDefaultAsync();

            if (productCategory != null)
            {
                var productViewModel = new ProductViewModel
                {
                    Product = product,
                    ProductCategoryName = productCategory.Title
                };

                productViewModels.Add(productViewModel);
            }
        }

        // Return the list of ProductViewModels
        return View(productViewModels);
    }


}
