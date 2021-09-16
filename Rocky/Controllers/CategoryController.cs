using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
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

           return View();

        }
        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                //return View();
                //redirection  !!
                /*============
                 * Here to re-digest 302 Redirection
                 * When a Post Request was sent to "Create"
                 * The server did sth ( insert data to DB)
                 * How to respond to this request, we have several approaches
                 * Redirection is one approach which tell the client browser 
                 * to request different resource
                 * In this case, the server tell the client to view the result
                 * of the POST request.
                 * ============*/
                return RedirectToAction("index");
            }
            else
            {
                // The validation is server-side validation
                // Those Error information will display only we respond this view
                // Those Errow information are not been activated when input so that they are not Client-side validation
                return View(obj);
            }
            

        }


        // Create a new Category
        public IActionResult Edit(int? key)
        {
            if(key==null||key==0)
            {
                return NotFound();
            }

            var obj = _db.Category.Find(key);
            if (obj == null)
            {
                return NotFound();
            }
            // pass this specified object to View
            return View(obj);

        }


        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(obj);
                _db.SaveChanges();
                //return View();
                //redirection  !!
                /*============
                 * Here to re-digest 302 Redirection
                 * When a Post Request was sent to "Create"
                 * The server did sth ( insert data to DB)
                 * How to respond to this request, we have several approaches
                 * Redirection is one approach which tell the client browser 
                 * to request different resource
                 * In this case, the server tell the client to view the result
                 * of the POST request.
                 * ============*/
                return RedirectToAction("index");
            }
            else
            {
                // The validation is server-side validation
                // Those Error information will display only we respond this view
                // Those Errow information are not been activated when input so that they are not Client-side validation
                return View(obj);
            }


        }


        // Create a new Category
        public IActionResult Delete(int? key)
        {

            if (key == null || key == 0)
            {
                return NotFound();
            }

            var obj = _db.Category.Find(key);
            if (obj == null)
            {
                return NotFound();
            }
            // pass this specified object to View
            return View(obj);


        }

        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category obj)
        {

                _db.Category.Remove(obj);
                _db.SaveChanges();
                //return View();
                //redirection  !!
                /*============
                 * Here to re-digest 302 Redirection
                 * When a Post Request was sent to "Create"
                 * The server did sth ( insert data to DB)
                 * How to respond to this request, we have several approaches
                 * Redirection is one approach which tell the client browser 
                 * to request different resource
                 * In this case, the server tell the client to view the result
                 * of the POST request.
                 * ============*/
                return RedirectToAction("index");


        }
    }

  
}
