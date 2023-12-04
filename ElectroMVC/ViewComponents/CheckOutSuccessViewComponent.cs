using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectroMVC.Data;
using Microsoft.AspNetCore.Mvc;
using ElectroMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ElectroMVC.Migrations;
using System.ComponentModel;


public class CheckOutSuccessViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CheckOutSuccessViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        //var item = _context.
        return View();
    }

    
}
