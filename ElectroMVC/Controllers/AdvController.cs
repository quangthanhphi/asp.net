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
    public class AdvController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adv
        public async Task<IActionResult> Index()
        {
              return _context.Adv != null ? 
                          View(await _context.Adv.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Adv'  is null.");
        }

        // GET: Adv/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Adv == null)
            {
                return NotFound();
            }

            var adv = await _context.Adv
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adv == null)
            {
                return NotFound();
            }

            return View(adv);
        }

        // GET: Adv/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adv/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Image,Link,Type,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Adv adv)
        {
            if (ModelState.IsValid)
            {
                adv.CreatedDate = DateTime.Now;
                adv.ModifiedDate = DateTime.Now;
                _context.Add(adv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adv);
        }

        // GET: Adv/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Adv == null)
            {
                return NotFound();
            }

            var adv = await _context.Adv.FindAsync(id);
            if (adv == null)
            {
                return NotFound();
            }
            return View(adv);
        }

        // POST: Adv/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Image,Link,Type,CreatedBy,CreatedDate,ModifiedDate,ModifiedBy")] Adv adv)
        {
            if (id != adv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvExists(adv.Id))
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
            return View(adv);
        }

        // GET: Adv/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Adv == null)
            {
                return NotFound();
            }

            var adv = await _context.Adv
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adv == null)
            {
                return NotFound();
            }

            return View(adv);
        }

        // POST: Adv/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Adv == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Adv'  is null.");
            }
            var adv = await _context.Adv.FindAsync(id);
            if (adv != null)
            {
                _context.Adv.Remove(adv);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvExists(int id)
        {
          return (_context.Adv?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
