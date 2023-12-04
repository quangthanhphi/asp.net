// ProductsViewComponent.cs

using System.Threading.Tasks;
using ElectroMVC.Data;
using ElectroMVC.Migrations;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectroMVC.ViewComponents
{
    public class OrderPartialViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public OrderPartialViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var items = _context.OrderDetail.Where(x => x.OrderId == id).ToList();

            var orderView = new List<OrderViewModel>();

            foreach(var item in items)
            {
                var product = _context.Product.Where(x => x.Id == item.ProductId).FirstOrDefault();
                if (product != null)
                {
                    var orderViewModel = new OrderViewModel
                    {
                        OrderDetail = item,
                        ProductName = product.Title
                    };

                    orderView.Add(orderViewModel);
                }
            }
            return View(orderView);
        }
        
    }
}
