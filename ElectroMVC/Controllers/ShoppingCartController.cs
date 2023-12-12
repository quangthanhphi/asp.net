using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Keep this for Entity Framework Core
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ElectroMVC.Migrations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ElectroMVC.Models.Payments;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


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
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShoppingCartController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public ActionResult VNpayReturn()
        {
            try
            {
                if (Request.Query.Count() > 0)
                {
                    string vnp_HashSecret = _configuration["VNPAY:HashSecret"]; //Chuoi bi mat
                    var vnpayData = Request.Query;
                    VnPayLibrary vnpay = new VnPayLibrary();

                    foreach (var key in vnpayData.Keys)
                    {
                        var value = vnpayData[key];
                        // get all querystring data
                        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(key, value);
                        }
                    }

                    string orderCode= Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                    long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                    string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                    string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                    long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                    String vnp_SecureHash = Request.Query["vnp_SecureHash"].FirstOrDefault();
                    String TerminalID = Request.Query["vnp_TmnCode"].FirstOrDefault();
                    String bankCode = Request.Query["vnp_BankCode"].FirstOrDefault();


                    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    if (checkSignature)
                    {
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            var itemOrder = _context.Order.FirstOrDefault(x => x.Code == orderCode);
                            if (itemOrder != null)
                            {
                                itemOrder.Status = 2;//Đã thanh toán
                                _context.Order.Attach(itemOrder);
                                _context.Entry(itemOrder).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _context.SaveChanges();
                            }
                            ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                            // log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                        }
                        else
                        {
                            ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                            // log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                        }
                        //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                        ViewBag.MaGiaoDich = "Mã giao dịch thanh toán:" + orderCode.ToString();
                        ViewBag.MaGiaoDichVNP = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                        ViewBag.NganHang = "Ngân hàng thanh toán:" + bankCode;

                    }
                }
            }
            catch (Exception ex)
            {
              Console.WriteLine( "An error occurred: " + ex.Message);
                // Log the exception if needed
            }

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

            var code = new { Success = false, Code = -1 ,Url = ""};
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
                    order.Email = req.Order.Email;
                    order.Status = 1; //chưa thanh toán , 2/đã thanh toán, 3/hoàn thành 4/hủy
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

                    code = new { Success = true, Code = req.Order.TypePayment, Url = "" };

                    //var url = "";
                    if (req.Order.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, order.Code);
                        code = new { Success = true, Code = req.Order.TypePayment, Url = url };
                    }
                    //return RedirectToAction("CheckOutSuccess");
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

       


            #region Thanh toán vnpay
            public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = _context.Order.FirstOrDefault(x => x.Code == orderCode);
            Console.WriteLine("Code của order " + order.Code);
            //Get Config Info
            string vnp_Returnurl = _configuration["VNPAY:ReturnUrl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["VNPAY:Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["VNPAY:TmnCode"]; //Ma website
            string vnp_HashSecret = _configuration["VNPAY:HashSecret"]; //Chuoi bi mat

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100 * 24000;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            Console.WriteLine("Generated VNPAY URL: " + urlPayment);
            Console.WriteLine(" VNPAY URL trả về: " + vnp_Returnurl);
            return urlPayment;
        }
        #endregion
    }
}
