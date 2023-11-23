using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElectroMVC.Data;
using ElectroMVC.Models;
using Microsoft.AspNetCore.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.Extensions.Hosting.Internal;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using ElectroMVC.Migrations;




namespace ElectroMVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.News.Include(n => n.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Detail,Image,ImageFile,CategoryId,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Models.News news, IFormFile imageFile)
        {
              
            if (ModelState.IsValid)
            {
                // Process other properties...

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

                    news.Image = "/images/" + uniqueFileName; // Update this based on your project structure
                }
                news.CreatedDate = DateTime.Now;
                news.ModifiedDate = DateTime.Now;

                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", news.CategoryId);
            return View(news);
        }


        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", news.CategoryId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Detail,Image,ImageFile,CategoryId,SeoTitle,SeoDescription,SeoKeywords,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Models.News news)
        {
            
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if a new image is provided
                if (news.ImageFile != null && news.ImageFile.Length > 0)
                {
                    // Delete old image if it exists
                    if (Path.GetFileName(news.Image) != null)
                    {
                        var oldImagePath = Path.Combine("wwwroot", "images", Path.GetFileName(news.Image));
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
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(news.ImageFile.FileName);
                    var newImagePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await news.ImageFile.CopyToAsync(stream);
                    }

                    // Update the news object with the new image path
                    news.Image = "/images/" + uniqueFileName; // Update this based on your project structure
                }

                    news.CreatedDate = DateTime.Now;
                    news.ModifiedDate = DateTime.Now;
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
               
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", news.CategoryId);
            return View(news);
        }


        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'ApplicationDbContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
