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
    public class SystemSettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemSetting
        public async Task<IActionResult> Index()
        {
              return _context.SystemSetting != null ? 
                          View(await _context.SystemSetting.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SystemSetting'  is null.");
        }

        // GET: SystemSetting/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SystemSetting == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSetting
                .FirstOrDefaultAsync(m => m.SettingKey == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // GET: SystemSetting/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemSetting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SettingKey,SettingValue,SettingDescription")] SystemSetting systemSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(systemSetting);
        }

        // GET: SystemSetting/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SystemSetting == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSetting.FindAsync(id);
            if (systemSetting == null)
            {
                return NotFound();
            }
            return View(systemSetting);
        }

        // POST: SystemSetting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SettingKey,SettingValue,SettingDescription")] SystemSetting systemSetting)
        {
            if (id != systemSetting.SettingKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemSettingExists(systemSetting.SettingKey))
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
            return View(systemSetting);
        }

        // GET: SystemSetting/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SystemSetting == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSetting
                .FirstOrDefaultAsync(m => m.SettingKey == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // POST: SystemSetting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SystemSetting == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SystemSetting'  is null.");
            }
            var systemSetting = await _context.SystemSetting.FindAsync(id);
            if (systemSetting != null)
            {
                _context.SystemSetting.Remove(systemSetting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemSettingExists(string id)
        {
          return (_context.SystemSetting?.Any(e => e.SettingKey == id)).GetValueOrDefault();
        }
    }
}
