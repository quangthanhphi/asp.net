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
    public class SubscribeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscribeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subscribe
        public async Task<IActionResult> Index()
        {
              return _context.Subscribe != null ? 
                          View(await _context.Subscribe.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Subscribe'  is null.");
        }

        // GET: Subscribe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subscribe == null)
            {
                return NotFound();
            }

            var subscribe = await _context.Subscribe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscribe == null)
            {
                return NotFound();
            }

            return View(subscribe);
        }

        // GET: Subscribe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscribe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,CreatedDate")] Subscribe subscribe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscribe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscribe);
        }

        // GET: Subscribe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subscribe == null)
            {
                return NotFound();
            }

            var subscribe = await _context.Subscribe.FindAsync(id);
            if (subscribe == null)
            {
                return NotFound();
            }
            return View(subscribe);
        }

        // POST: Subscribe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,CreatedDate")] Subscribe subscribe)
        {
            if (id != subscribe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscribe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscribeExists(subscribe.Id))
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
            return View(subscribe);
        }

        // GET: Subscribe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subscribe == null)
            {
                return NotFound();
            }

            var subscribe = await _context.Subscribe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subscribe == null)
            {
                return NotFound();
            }

            return View(subscribe);
        }

        // POST: Subscribe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subscribe == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Subscribe'  is null.");
            }
            var subscribe = await _context.Subscribe.FindAsync(id);
            if (subscribe != null)
            {
                _context.Subscribe.Remove(subscribe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscribeExists(int id)
        {
          return (_context.Subscribe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
