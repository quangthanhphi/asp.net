using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using ElectroMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
namespace ElectroMVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Countc = 0 };
            var checkProduct = _context.Product.FirstOrDefault(x => x.Id == id);

            if (checkProduct != null)
            {
                ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                if (cart.Items == null)
                {
                    cart.Items = new List<ShoppingCartItem>();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }

                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    ProductName = checkProduct.Title,
                    Quantity = quantity
                };

                // Check if ProductCategory is not null before accessing its properties
                if (checkProduct.ProductCategory != null)
                {
                    item.CategoryName = checkProduct.ProductCategory.Title;
                }

                // Check if Image is not null before accessing its properties
                if (checkProduct.Image != null)
                {
                    item.ProductImg = checkProduct.Image;
                }

                item.Price = checkProduct.Price;

                if (checkProduct.PriceSale > 0)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }

                item.TotalPrice = item.Quantity * item.Price;

                cart.AddToCart(item, quantity);

                HttpContext.Session.SetObjectAsJson("Cart", cart);
                code = new { Success = true, msg = "Thêm sản phẩm thành công", code = 1, Countc = cart.Items.Count };
                
                Console.WriteLine("Cart Item:" + cart.Items.Count);
            }

            return Json(code);
        }

    }
}
