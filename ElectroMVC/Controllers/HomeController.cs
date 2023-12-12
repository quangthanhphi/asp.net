 using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ElectroMVC.Models;

namespace ElectroMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Index1()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult AllProducts()
    {
        return View();
    }
    public IActionResult PCategory()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

