using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElectroMVC.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Fetch recent orders from the database
            var item = _context.Order
                .OrderByDescending(o => o.CreatedDate)
                .Take(5) // Adjust the number based on your preference
                .ToList();
            return View(item);
        }

       
    }


}

