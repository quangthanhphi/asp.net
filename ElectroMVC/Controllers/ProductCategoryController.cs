using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElectroMVC.Data;
using ElectroMVC.Models;

namespace ElectroMVC.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductCategory
        public async Task<IActionResult> Index()
        {
              return _context.ProductCategory != null ? 
                          View(await _context.ProductCategory.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ProductCategory'  is null.");
        }

        // GET: ProductCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductCategory == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductCategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Icon,ImageFile,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] ProductCategory productCategory, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                //Xử lí hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Example: Save to the wwwroot/images folder
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    productCategory.Icon = "/images/" + uniqueFileName; // Update this based on your project structure
                }

                productCategory.Alias = ElectroMVC.Models.Filter.FilterChar(productCategory.Title);
                productCategory.CreatedDate = DateTime.Now;
                productCategory.ModifiedDate = DateTime.Now;
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductCategory == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Icon,ImageFile,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new image is provided
                    if (productCategory.ImageFile != null && productCategory.ImageFile.Length > 0)
                    {
                        // Delete old image if it exists
                        if (Path.GetFileName(productCategory.Icon) != null)
                        {
                            var oldImagePath = Path.Combine("wwwroot", "images", Path.GetFileName(productCategory.Icon));
                            // Delete old image if it exists
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            //Console.WriteLine("Old Image Path: " + oldImagePath); // Add this line for debugging

                        }


                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");


                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Save the new image
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(productCategory.ImageFile.FileName);
                        var newImagePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(newImagePath, FileMode.Create))
                        {
                            await productCategory.ImageFile.CopyToAsync(stream);
                        }

                        // Update the news object with the new image path
                        productCategory.Icon = "/images/" + uniqueFileName; // Update this based on your project structure
                    }
                    productCategory.Alias = ElectroMVC.Models.Filter.FilterChar(productCategory.Title);
                    productCategory.CreatedDate = DateTime.Now;
                    productCategory.ModifiedDate = DateTime.Now;
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
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
            return View(productCategory);
        }

        // GET: ProductCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductCategory == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductCategory == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProductCategory'  is null.");
            }
            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory != null)
            {
                // Delete old image if it exists
                if (Path.GetFileName(productCategory.Icon) != null)
                {
                    var oldImagePath = Path.Combine("wwwroot", "images", Path.GetFileName(productCategory.Icon));
                    // Delete old image if it exists
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    //Console.WriteLine("Old Image Path: " + oldImagePath); // Add this line for debugging

                }

                _context.ProductCategory.Remove(productCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCategoryExists(int id)
        {
          return (_context.ProductCategory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
