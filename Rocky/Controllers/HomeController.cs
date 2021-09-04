using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky.Examples.DI;
using Rocky.Models;
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
        
        private readonly IMyDependency _myDependency;     // test DI

        public HomeController(ILogger<HomeController> logger, IMyDependency myDependency)
        {
            _logger = logger;
            _myDependency = myDependency;
        }

        public IActionResult Index()
        {
            
            //  Testing DI
            _myDependency.WriteMessage("TESTING Dependency Injection !!!");

            //  Test get TYPE information of Class Category

            Type categoryType = typeof(Category);
            PropertyInfo[] propertyList = categoryType.GetProperties();


            return View();
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
