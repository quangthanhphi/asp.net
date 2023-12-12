using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElectroMVC.Controllers
{
    public class RoleController : Controller
    {

        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var items = _context.Roles.ToList();
            return View(items);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(_context),
                    null, // Pass null for roleValidators
                    null, // Pass null for keyNormalizer
                    null, // Pass null for errors
                    null  // Pass null for logger
                );

                roleManager.CreateAsync(model).GetAwaiter().GetResult(); // Use CreateAsync and wait for the result
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var item = _context.Roles.Find(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(_context),
                    null, // Pass null for roleValidators
                    null, // Pass null for keyNormalizer
                    null, // Pass null for errors
                    null  // Pass null for logger
                );

                roleManager.UpdateAsync(model).GetAwaiter().GetResult(); // Use CreateAsync and wait for the result
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

