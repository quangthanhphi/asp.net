using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElectroMVC.Data;
using ElectroMVC.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace ElectroMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public ActionResult Index(int? page)
        {
            var item = _context.Order.OrderByDescending(x => x.CreatedDate).ToList();
            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 5;

            DateTime currentDate = DateTime.Now;
            decimal totalRevenueThisMonth = _context.Order
                .Where(o => o.CreatedDate.Month == currentDate.Month && o.CreatedDate.Year == currentDate.Year)
                .Sum(o => o.TotalAmount);

            ViewBag.TotalRevenueThisMonth = totalRevenueThisMonth;

            decimal totalRevenueThisYear = _context.Order
                .Where(o => o.CreatedDate.Year == currentDate.Year)
                .Sum(o => o.TotalAmount);

            ViewBag.TotalRevenueThisYear = totalRevenueThisYear;

            int totalOrdersThisMonth = _context.Order
        .Count(o => o.CreatedDate.Year == currentDate.Year && o.CreatedDate.Month == currentDate.Month);

            int totalOrdersThisYear = _context.Order
                .Count(o => o.CreatedDate.Year == currentDate.Year);

            ViewBag.TotalOrdersThisMonth = totalOrdersThisMonth;
            ViewBag.TotalOrdersThisYear = totalOrdersThisYear;


            return View(item.ToPagedList(pageNumber, pageSize));
            //return _context.Order != null ? 
            //            View(await _context.Order.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.Order'  is null.");
        }
        public IActionResult GetOrderInfo(string orderCode)
        {
            var orderInfo = GetOrderInfoFromDatabase(orderCode);

            // Tạo HTML dựa trên thông tin đơn hàng
            var htmlResult = $@"
       
    <table align='center' border='1' cellspacing='0'>
            <tr>
                <th colspan='2'>
            Thông tin đơn hàng {orderInfo.Code}
            {(orderInfo.Status != 4 ? "<button id='cancelButton' class='btn btn-warning btn-sm' onclick='confirmCancel(" + orderInfo.Id + ")'>Hủy</button>" : "")}
                </th>
            </tr>
            <tr>
                <th>Thông tin khách hàng</th>
                <td>
                     Khách hàng: {orderInfo.CustomerName}<br>
                     Email: {orderInfo.Email}<br>
                     SĐT: {orderInfo.Phone}<br>
                     Địa chỉ: {orderInfo.Address}<br>
                </td>
            </tr>
            <tr>
                <th>Thông tin đơn hàng</th>
                <td>
                    Thời gian mua hàng: {orderInfo.CreatedDate.ToString("HH:mm dd/MM/yyyy")} <br>
                    Tổng tiền: ${orderInfo.TotalAmount} <br>
                </td>
            </tr>
            <tr>
                <th>Trạng thái</th>
                
                <td>
    {(orderInfo.Status == 1 ? "<text style='color: red;'>Chưa thanh toán</text>" :
       orderInfo.Status == 2 ? "<text style='color: green;'>Đã thanh toán</text>" :
       orderInfo.Status == 3 ? "<text style='color: blue;'>Hoàn thành</text>" :
                               "<b style='color: red;'>Hủy</b>")}
                </td>


               
            </tr>
        </table>
        
    ";

            // Trả về dữ liệu HTML
            return Content(htmlResult, "text/html");
        }



        private Order GetOrderInfoFromDatabase(string orderCode)
        {
            var item = _context.Order.FirstOrDefault(x => x.Code == orderCode);


            return item;
        }

        public IActionResult GetOrderViewInfo(string orderCode)
        {
            var orderInfoList = GetOrderViewInfoFromDatabase(orderCode);

            
            // Tạo HTML dựa trên thông tin đơn hàng
            var htmlResult = "<div class='text-center'>";
            var i = 1;
            foreach (var orderInfo in orderInfoList)
            {
                if (orderInfo.Order.Status != 4)
                {
                    string[] arr = orderInfo.Product.Image.Split(';');
                    htmlResult += $@"
                <div style='display: inline-block; margin: 10px;'>
                    <h5>Thông tin sản phẩm {i}</h5>
                    <div>
                        <img src='{arr[0]}' alt='Product Image' style='max-width: 100px; height:100px;' /><br>
                        {orderInfo.ProductName}<br>
                        Giá: ${orderInfo.OrderDetail.Price}<br>
                        Tổng tiền: ${orderInfo.OrderDetail.Price * orderInfo.OrderDetail.Quantity}<br>
                        Số lượng: <input class='quantity-input' id='quantityInput' value='{orderInfo.OrderDetail.Quantity}' />
            ";

                    if (orderInfo.Order.Status == 1)
                    {
                        htmlResult += $@"
                <button class='btn btn-danger btn-sm' onclick='confirmDelete({orderInfo.OrderDetail.OrderId},{orderInfo.OrderDetail.ProductId})'>Hủy</button>
                    ";
                    }

                    htmlResult += $@"
                    </div>
                </div>
            ";

                    i++;
                }

            }

            htmlResult += "</div>";

            // Trả về dữ liệu HTML
            return Content(htmlResult, "text/html");
        }

        private List<OrderViewModel> GetOrderViewInfoFromDatabase(string orderCode)
        {
            // Tìm đơn hàng với mã orderCode
            var order = _context.Order.FirstOrDefault(x => x.Code == orderCode);

            // Kiểm tra xem order có tồn tại không
            if (order == null)
            {
                // Trả về danh sách rỗng nếu không tìm thấy order
                return new List<OrderViewModel>();
            }

            // Lấy chi tiết đơn hàng liên quan
            var orderDetails = _context.OrderDetail.Where(x => x.OrderId == order.Id).ToList();

            // Tạo danh sách OrderViewModel
            List<OrderViewModel> items = orderDetails.Select(orderDetail =>
            {
                // Lấy thông tin sản phẩm từ orderDetail
                var product = _context.Product.FirstOrDefault(p => p.Id == orderDetail.ProductId);

                // Tạo một đối tượng OrderViewModel mới
                return new OrderViewModel
                {
                    Order = order,
                    OrderDetail = orderDetail,
                    Product = product,
                    ProductName = product?.Title, // Gán tên sản phẩm vào ProductName
                    
        };
            }).ToList();

            return items;
        }

        public IActionResult GetOrderInfoID(int orderId)
        {
            var orderInfo = GetOrderInfoFromDatabaseID(orderId);

            // Tạo HTML dựa trên thông tin đơn hàng
            var htmlResult = $@"
       
    <table align='center' border='1' cellspacing='0'>
            <tr>
                <th colspan='2'>Thông tin đơn hàng {orderInfo.Code}
                      {(orderInfo.Status != 4 ? "<button id='cancelButton' class='btn btn-warning btn-sm' onclick='confirmCancel(" + orderInfo.Id + ")'>Hủy</button>" : "")}
                </th>
            </tr>
            <tr>
                <th>Thông tin khách hàng</th>
                <td>
                     Khách hàng: {orderInfo.CustomerName}<br>
                     Email: {orderInfo.Email}<br>
                     SĐT: {orderInfo.Phone}<br>
                     Địa chỉ: {orderInfo.Address}<br>
                </td>
            </tr>
            <tr>
                <th>Thông tin đơn hàng</th>
                <td>
                    Thời gian mua hàng: {orderInfo.CreatedDate.ToString("HH:mm dd/MM/yyyy")} <br>
                    Tổng tiền: ${orderInfo.TotalAmount} <br>
                </td>
            </tr>
            <tr>
                <th>Trạng thái</th>
                
                <td>
    {(orderInfo.Status == 1 ? "<text style='color: red;'>Chưa thanh toán</text>" :
       orderInfo.Status == 2 ? "<text style='color: green;'>Đã thanh toán</text>" :
       orderInfo.Status == 3 ? "<text style='color: blue;'>Hoàn thành</text>" :
                               "<b style='color: red;'>Hủy</b>")}
                
                </td>

               
            </tr>
        </table>
        
    ";

            // Trả về dữ liệu HTML
            return Content(htmlResult, "text/html");
        }



        private Order GetOrderInfoFromDatabaseID(int orderId)
        {
            var item = _context.Order.FirstOrDefault(x => x.Id == orderId);


            return item;
        }

        public IActionResult GetOrderViewInfoID(int orderId)
        {
            var orderInfoList = GetOrderViewInfoFromDatabaseID(orderId);

            // Tạo HTML dựa trên thông tin đơn hàng
            var htmlResult = "<div class='text-center'>";
            var i = 1;
            foreach (var orderInfo in orderInfoList)
            {
                if (orderInfo.Order.Status != 4)
                {
                    string[] arr = orderInfo.Product.Image.Split(';');
                    htmlResult += $@"
                <div style='display: inline-block; margin: 10px;'>
                    <h5>Thông tin sản phẩm {i}</h5>
                    <div>
                        <img src='{arr[0]}' alt='Product Image' style='max-width: 100px;height:100px;' /><br>
                        {orderInfo.ProductName}<br>
                        Giá: ${orderInfo.OrderDetail.Price}<br>
                        Tổng tiền: ${orderInfo.OrderDetail.Price * orderInfo.OrderDetail.Quantity}<br>
                        Số lượng: <input class='quantity-input' id='quantityInput' value='{orderInfo.OrderDetail.Quantity}' />
            ";

                    if (orderInfo.Order.Status == 1)
                    {
                        htmlResult += $@"
                <button class='btn btn-danger btn-sm' onclick='confirmDelete({orderInfo.OrderDetail.OrderId},{orderInfo.OrderDetail.ProductId})'>Hủy</button>
                    ";
                    }

                    htmlResult += $@"
                    </div>
                </div>
            ";

                    i++;
                }
            }

            htmlResult += "</div>";

            // Trả về dữ liệu HTML
            return Content(htmlResult, "text/html");
        }

        private List<OrderViewModel> GetOrderViewInfoFromDatabaseID(int orderId)
        {
            // Tìm đơn hàng với mã orderCode
            var order = _context.Order.FirstOrDefault(x => x.Id == orderId);

            // Kiểm tra xem order có tồn tại không
            if (order == null)
            {
                // Trả về danh sách rỗng nếu không tìm thấy order
                return new List<OrderViewModel>();
            }

            // Lấy chi tiết đơn hàng liên quan
            var orderDetails = _context.OrderDetail.Where(x => x.OrderId == order.Id).ToList();

            // Tạo danh sách OrderViewModel
            List<OrderViewModel> items = orderDetails.Select(orderDetail =>
            {
                // Lấy thông tin sản phẩm từ orderDetail
                var product = _context.Product.FirstOrDefault(p => p.Id == orderDetail.ProductId);

                // Tạo một đối tượng OrderViewModel mới
                return new OrderViewModel
                {
                    Order = order,
                    OrderDetail = orderDetail,
                    Product = product,
                    ProductName = product?.Title, // Gán tên sản phẩm vào ProductName

                };
            }).ToList();

            return items;
        }

        //Update số lượng
        [HttpPost]
        public IActionResult UpdateOrderDetailQuantity(int orderId, int productId, int newQuantity)
        {
            try
            {
                var orderDetail = _context.OrderDetail.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);

                if (orderDetail != null)
                {
                    // Cập nhật số lượng trong đối tượng OrderDetail
                    var newTotal = orderDetail.Price * newQuantity - orderDetail.Price * orderDetail.Quantity;
                    orderDetail.Quantity = newQuantity;

                    // Cập nhật lại tổng tiền trong đối tượng Order
                    var order = _context.Order.FirstOrDefault(o => o.Id == orderId);
                    //order.TotalAmount = _context.OrderDetail.Where(od => od.OrderId == orderId);

                    order.TotalAmount += newTotal;
                    

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        //Hết update

        // Hàm để lấy danh sách đơn hàng
        [HttpGet]
        public IActionResult GetOrderList()
        {
            try
            {

                var orders = _context.Order.OrderByDescending(o => o.CreatedDate).ToList();


                // Chuyển đổi danh sách đơn hàng thành HTML
                var orderTableHtml = "<table class='table' border='1' cellspacing='0'><tr><th>Mã đơn hàng</th><th>Ngày tạo</th><th>Khách hàng</th><th>Trạng thái</th></tr>";

                foreach (var order in orders)
                {
                    orderTableHtml += $@"
                <tr>
                        <td>{order.Code}</td><td>{order.CreatedDate.ToString("HH:mm dd-MM-yyyy")}</td><td>{order.CustomerName}</td> 
                        <td>
    {(order.Status == 1 ? "<text style='color: red;'>Chưa thanh toán</text>" :
       order.Status == 2 ? "<text style='color: green;'>Đã thanh toán</text>" :
       order.Status == 3 ? "<text style='color: blue;'>Hoàn thành</text>" :
                               "<b style='color: red;'>Hủy</b>")}
    

                        </ td >

                </tr>";
                }

                orderTableHtml += "</table>";

                // Trả về dữ liệu HTML với kiểu text/html
                return Content(orderTableHtml, "text/html");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrderList: {ex.Message}");
                // Xử lý lỗi và trả về response lỗi
                return BadRequest(new { error = ex.Message });
            }
        }



        //XÓa order detail
        [HttpPost]
        public IActionResult DeleteOrderDetail(int orderId, int productId)
        {
            try
            {
                var orderDetail = _context.OrderDetail.FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);

                if (orderDetail != null)
                {
                    // Lấy giá trị của orderDetail trước khi xóa
                    var deletedPrice = orderDetail.Price;
                    var deleteQuantity = orderDetail.Quantity;

                    _context.OrderDetail.Remove(orderDetail);
                    _context.SaveChanges();

                    // Cập nhật tổng tiền trong order sau khi xóa
                    var order = _context.Order.Find(orderId);
                    order.TotalAmount = order.TotalAmount - deletedPrice*deleteQuantity;
                    _context.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Không tìm thấy chi tiết đơn hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        //Xong xóa

        //Hủy đơn
        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                var order = _context.Order.Find(orderId);

                if (order != null)
                {
                    // Cập nhật trạng thái đơn hàng thành Hủy (Status = 4)
                    order.Status = 4;
                    _context.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        //Xong hủy đơn

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public ActionResult Partial(int id)
        {
            return ViewComponent("OrderPartial", id);
        }

        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = _context.Order.Find(id);
            if (item != null)
            {
                _context.Order.Attach(item);
                item.Status = trangthai;
                _context.Entry(item).Property(x => x.TypePayment).IsModified = true;
                _context.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Fail", Success = false });

        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,CustomerName,Phone,Address,Quantity,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,CustomerName,Phone,Address,Quantity,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
