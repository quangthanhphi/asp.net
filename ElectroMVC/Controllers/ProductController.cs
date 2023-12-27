using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;
using ElectroMVC.Data;
using ElectroMVC.Models;

namespace ElectroMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(int? page)
        {
            var products = _context.Product.Include(p => p.ProductCategory);
            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var paginatedProducts = await products.ToPagedListAsync(pageNumber, pageSize);

            return View(paginatedProducts);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var productViewModel = new List<ProductViewModel>();

            // Lấy thông tin sản phẩm với ProductCategory thông qua mối quan hệ Include
            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy thông tin ProductCategoryName
            var productCategory = await _context.ProductCategory
                .Where(pc => pc.Id == product.ProductCategoryId)
                .FirstOrDefaultAsync();

            if (productCategory != null)
            {
                productViewModel.Add(new ProductViewModel
                {
                    Product = product,
                    ProductCategoryName = productCategory.Title
                });
            }

            // Set ProductCategoryName in ViewBag
            ViewBag.ProductCategoryName = productCategory?.Title;

            return View(product);
        }



        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.Set<ProductCategory>(), "Id", "Title");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ProductCode,Description,Detail,Image,ImageFiles,Price,PriceSale,Quantity,IsHome,IsSale,IsFeature,IsHot,ProductCategoryId,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Product product, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có giá trị được chọn cho ProductCategoryId hay không
                if (product.ProductCategoryId == 0)
                {
                    ModelState.AddModelError("ProductCategoryId", "The ProductCategory field is required.");
                    ViewData["ProductCategoryId"] = new SelectList(_context.Set<ProductCategory>(), "Id", "Title", product.ProductCategoryId);
                    return View(product);
                }

                if (imageFiles.Count > 0)
                {
                    var imagePaths = new List<string>();

                    foreach (var file in imageFiles)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        imagePaths.Add("/images/" + uniqueFileName);
                    }

                    // Nối đường dẫn của ảnh và ngăn chúng bởi dấu chấm phẩy
                    product.Image = string.Join(";", imagePaths);
                }

                product.Alias = ElectroMVC.Models.Filter.FilterChar(product.Title);
                product.CreatedDate = DateTime.Now;
                product.ModifiedDate = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductCategoryId"] = new SelectList(_context.Set<ProductCategory>(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }


        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.Set<ProductCategory>(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ProductCode,Description,Detail,Image,ImageFiles,Price,PriceSale,Quantity,IsHome,IsSale,IsFeature,IsHot,ProductCategoryId,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new image is provided
                    if (product.ImageFiles != null && product.ImageFiles.Count > 0)
                    {
                        var imagePaths = new List<string>();
                        // Delete old image if it exists
                        if (Path.GetFileName(product.Image) != null)
                        {
                            string[] arr = product.Image.Split(';');
                            foreach (var img in arr)
                            {
                                var oldImagePath = Path.Combine("wwwroot", "images", Path.GetFileName(img));
                                // Delete old image if it exists
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                                //Console.WriteLine("Old Image Path: " + oldImagePath); // Add this line for debugging

                            }
                        }
                            foreach (var file in product.ImageFiles)
                        {
       
                                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }

                                imagePaths.Add("/images/" + uniqueFileName);
                            }

                            // Nối đường dẫn của ảnh và ngăn chúng bởi dấu chấm phẩy
                            product.Image = string.Join(";", imagePaths);
                        }
                    product.Alias = ElectroMVC.Models.Filter.FilterChar(product.Title);
                    product.CreatedDate = DateTime.Now;
                    product.ModifiedDate = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductCategoryId"] = new SelectList(_context.Set<ProductCategory>(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                // Delete old image if it exists
                if (Path.GetFileName(product.Image) != null)
                {
                    string[] arr = product.Image.Split(';');
                    foreach (var img in arr)
                    {
                        var oldImagePath = Path.Combine("wwwroot", "images", Path.GetFileName(img));
                        // Delete old image if it exists
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        //Console.WriteLine("Old Image Path: " + oldImagePath); // Add this line for debugging
                    }
                }
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
