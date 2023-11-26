using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectroMVC.Controllers
{
    public class MenuController : Controller
    {

        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //public ActionResult MenuTop()
        //{
        //    var items = _context.Category.OrderBy(x => x.Position).ToList();
        //    return PartialView("~/Views/Menu/MenuTop.cshtml", items);
        //}



    }
}