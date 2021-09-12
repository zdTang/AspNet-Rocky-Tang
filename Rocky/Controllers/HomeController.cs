using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Examples.DI;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        
       /* private readonly IMyDependency _myDependency; */    // test DI

        public HomeController(ILogger<HomeController> logger, /*IMyDependency myDependency,*/ ApplicationDbContext db)
        {
            _logger = logger;
            //_myDependency = myDependency;
            _db = db;
        }

        public IActionResult Index()
        {

            //  Testing DI
            //_myDependency.WriteMessage("TESTING Dependency Injection !!!");

            //  Test get TYPE information of Class Category

            //Type categoryType = typeof(Category);
            //PropertyInfo[] propertyList = categoryType.GetProperties();
            HomeVM homeVM = new HomeVM()
            {
                //Category, ApplicationType will be used at _individualProductCard  partial Table
                Products = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType),
                Categories = _db.Category
            };

            return View(homeVM);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
