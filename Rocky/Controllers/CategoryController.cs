using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;                                          //  Dependency Injection
        }
        
        public IActionResult Index()
        {

            IEnumerable<Category> objList = _db.Category;     //  Grab a collection from DB
            
            
            return View(objList);
            //var content = new ContentResult();
            //content.Content = "hello";
            //return content;
        }

        // Create a new Category
        public IActionResult Create()
        {

                 //  Grab a collection from DB


            return View();

        }
    }
}
