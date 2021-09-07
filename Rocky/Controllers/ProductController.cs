using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;                                          //  Dependency Injection
        }
        
        public IActionResult Index()
        {

            IEnumerable<Product> objList = _db.Product;     //  Grab a collection from DB
            
            foreach(var obj in objList)
            {
                // Each Product has its Category object which represent with Foreign-key
                // use the foreign-key to access the content of foreign key 
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            }
            
            return View(objList);
            //var content = new ContentResult();
            //content.Content = "hello";
            //return content;
        }

        // Create a new Category
        public IActionResult upsert(int? id)
        {
            /* Approach one === Use ViewBag, ViewData, TempData to pass model
             * Which is loosely typed view
             * 
            // take Category content to make a dropdown list
            // To make a dropdown list, we need a value(for program) and text(for display)
            IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            // viewBag is dynamic type ==> no static binding during compile time !!
            //ViewBag.CategoryDropDown = CategoryDropDown;         //  use ViewBag
            ViewData["CategoryDropDown"]= CategoryDropDown;        //  use ViewData

            Product product = new Product();            
             */

            // Approach 2: Create a special view model and pass it via View()
            // So that we can have a strong typed view 

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };



            // If use press "create a new product button"
            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(productVM.Product);
                }
            }

        }
        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            //if (ModelState.IsValid)
            //{
            //    _db.Product.Add(obj);
            //    _db.SaveChanges();
            //    //return View();
            //    //redirection  !!
            //    /*============
            //     * Here to re-digest 302 Redirection
            //     * When a Post Request was sent to "Create"
            //     * The server did sth ( insert data to DB)
            //     * How to respond to this request, we have several approaches
            //     * Redirection is one approach which tell the client browser 
            //     * to request different resource
            //     * In this case, the server tell the client to view the result
            //     * of the POST request.
            //     * ============*/
            //    return RedirectToAction("index");
            //}
            //else
            //{
            //    // The validation is server-side validation
            //    // Those Error information will display only we respond this view
            //    // Those Errow information are not been activated when input so that they are not Client-side validation
            //    return View(obj);
            //}

            return Content("good");
        }


        // Create a new Category
        public IActionResult Edit(int? key)
        {
            if(key==null||key==0)
            {
                return NotFound();
            }

            var obj = _db.Product.Find(key);
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
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _db.Product.Update(obj);
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

            var obj = _db.Product.Find(key);
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
        public IActionResult Delete(Product obj)
        {

                _db.Product.Remove(obj);
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
