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
using ElectroMVC.Migrations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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


        public ActionResult Partial_Item_Cart()
        {
            return ViewComponent("Partial_Item_Cart");
        }

        public ActionResult CheckOut()
        {
            return ViewComponent("CheckOut");
        }

        public ActionResult CheckOutSuccess()
        {
            return ViewComponent("CheckOutSuccess");
        }


        public ActionResult ShowCount()
        {
            ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if(cart != null)
            {
                return Json(new { Countc = cart.Items.Count });
            }
            return Json(new { Countc = 0});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut1 (OrderViewModel req)
        {
            Console.WriteLine("Giá trị của req: " + req);
            Console.WriteLine("Giá trị của req Order: " + req.Order);
            Console.WriteLine("Giá trị của req OrderName: " + req.Order.CustomerName);
            Console.WriteLine("Giá trị của req Email: " + req.Order.Email);
            Console.WriteLine("Giá trị của req Address: " + req.Order.Address);
            Console.WriteLine("Giá trị của req Phone: " + req.Order.Phone);

            var code = new { Success = false, Code = -1 };
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Lỗi");
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("Hợp lệ");
                ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart != null && req.Order != null)
                {
                    Console.WriteLine("cart và order khác null");
                    Models.Order order = new Models.Order();
                    order.CustomerName = req.Order.CustomerName;
                    order.Phone = req.Order.Phone;
                    order.Address = req.Order.Address;  
                    cart.Items.ForEach(x => order.orderDetails.Add(new Models.OrderDetail
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    }));
                    Console.WriteLine("Số lượng: " + cart.Items.Count());
                    order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                    order.TypePayment = req.Order.TypePayment;
                    order.CreatedDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.CreatedBy = req.Order.Phone;
                    Random rd = new Random();
                    order.Code = "DH" + rd.Next(0,9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    _context.Add(order);
                    _context.SaveChanges();
                    cart.ClearCart();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                    return RedirectToAction("CheckOutSuccess");
                }
            }
            return Json(code);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Countc = 0 };
            var checkProduct = _context.Product.FirstOrDefault(x => x.Id == id);

            var productViewModels = new List<ProductViewModel>();

            
                var productCategory = await _context.ProductCategory
                    .Where(pc => pc.Id == checkProduct.ProductCategoryId)
                    .FirstOrDefaultAsync();

                if (productCategory != null)
                {
                    var productViewModel = new ProductViewModel
                    {
                        Product = checkProduct,
                        ProductCategoryName = productCategory.Title
                    };

                    productViewModels.Add(productViewModel);
                }
            

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

                item.CategoryName = productViewModels.FirstOrDefault()?.ProductCategoryName; ;

                //Console.WriteLine("ProductCategory Name: " + item.CategoryName);

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

                //Console.WriteLine("Cart Item:" + cart.Items.Count);
            }

            return Json(code);
        }

        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                return Json(new { Success = true, Countc = 0 }); ;
            }
            return Json(new { Success = false });

        }
        


        [HttpPost]
        public ActionResult Delete(int id )
        {
            var code = new { Success = false, msg = "", code = -1, Countc = 0 };
            ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var checkProduct = _context.Product.FirstOrDefault(x => x.Id == id);
                if(checkProduct != null)
                {
                    cart.Remove(id);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                    code = new { Success = true, msg = "", code = 1, Countc = cart.Items.Count };
                }
            }
            //Console.WriteLine("Đang xóa");
            return Json(code);
        }

        

        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if(cart != null)
            {
                cart.ClearCart();
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                return Json(new { Success = true, Countc = 0 }); ;
            }
            return Json(new { Success = false });

        }

    }
}
