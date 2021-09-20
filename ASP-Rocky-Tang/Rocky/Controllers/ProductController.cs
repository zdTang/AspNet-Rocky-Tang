using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {

        //private readonly ApplicationDbContext _db;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _db;
        private readonly IWebHostEnvironment _webHostEnvironment;    // see setup.cs, this IWebHostEnvironment is defined by system

        public ProductController(/*ApplicationDbContext*/IProductRepository db, IWebHostEnvironment webHostEnvironment,ILogger<ProductController> logger)
        {
            _db = db;            //  Dependency Injection
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _logger.LogWarning("instantiate-- ProductController");
            _logger.LogWarning(User?.Identity?.Name);
        }
    
        
        public IActionResult Index()
        {
            // grab the only product to display
            // IEnumerable<Product> objList = _db.Product;     //  Grab a collection from DB

            // grab product and foreign-key tables
            IEnumerable<Product> objList = _db.GetAll(includeProperties:"Category,ApplicationType");

            /*=======================          
            foreach(var obj in objList)
            {
                // Each Product has its Category object which represent with Foreign-key
                // use the foreign-key to access the content of foreign key 
                // DB connection String must have "MultipleActiveResultSets=True"  or here will have error
                obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            }        
            =========================*/


            return View(objList);
            //var content = new ContentResult();
            //content.Content = "hello";
            //return content;
        }

        // Create a new Category
        public IActionResult Upsert(int? Key)
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
                // for Category dropdown-list
                CategorySelectList = _db.GetAllDropdownList(WC.CategoryName),
                //  // for ApplicationType dropdown-list
                ApplicationTypeSelectList = _db.GetAllDropdownList(WC.ApplicationTypeName),
                
            };



            // Users click "create a new product button"
            // It is just GET request without any variable
            if (Key == null)
            {
                return View(productVM);                      //  Return a empty content to the update View
            }
            else
            {
                // If users click update button, it will pass an ID here
                
                productVM.Product = _db.Find(Key.GetValueOrDefault());    //  Get the content based on the ID
                
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    //return View(productVM.Product);         //  Return ID's content to the update View
                    return View(productVM);
                }
            }

        }
        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                
                var files = HttpContext.Request.Form.Files;             // HttpContext object
                
                string webRootPath = _webHostEnvironment.WebRootPath;   // IWebHostEnvironment is injected by the system
                if (productVM.Product.Id == 0)
                {
                    // be aware: the ID of new created record is useless as the it will increase automatically
                    // if this id==0, means it is a default value of ID
                    
                    // creating 
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);         // write from Memory to specified folder
                    }

                    productVM.Product.Image = fileName + extension;   //  record the file name and save to DB
                    _db.Add(productVM.Product);

                }
                else
                {
                    // updating
                    // We need to retrive the old image name 
                    // As we only need to get the image name, so that we just add "AsNoTracking()"
                    // Or the DbContext will tracking two IDs which will cause ERROR
                    // why noe just retrive image name here ???!!!
                    //var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

                    var objFromDb = _db.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking:false);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        // Delete old file
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);         // write from Memory to specified folder
                        }

                        productVM.Product.Image = fileName + extension;   //  record the file name and save to DB
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Update(productVM.Product);
                    
                }
                _db.Save();
                return RedirectToAction("index");
            }
            else
            {
                // Make sure the dropdown list can work
                // so that we must make the viewModel have complete data
                productVM.CategorySelectList = _db.GetAllDropdownList(WC.CategoryName);
                productVM.ApplicationTypeSelectList = _db.GetAllDropdownList(WC.ApplicationTypeName);
                return View(productVM);
            }
            

        }


        // Create a new Category
        //public IActionResult Edit(int? key)
        //{
        //    if(key==null||key==0)
        //    {
        //        return NotFound();
        //    }

        //    var obj = _db.Product.Find(key);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    // pass this specified object to View
        //    return View(obj);

        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Product.Update(obj);
        //        _db.SaveChanges();
        //        //return View();
        //        //redirection  !!
        //        /*============
        //         * Here to re-digest 302 Redirection
        //         * When a Post Request was sent to "Create"
        //         * The server did sth ( insert data to DB)
        //         * How to respond to this request, we have several approaches
        //         * Redirection is one approach which tell the client browser 
        //         * to request different resource
        //         * In this case, the server tell the client to view the result
        //         * of the POST request.
        //         * ============*/
        //        return RedirectToAction("index");
        //    }
        //    else
        //    {
        //        // The validation is server-side validation
        //        // Those Error information will display only we respond this view
        //        // Those Errow information are not been activated when input so that they are not Client-side validation
        //        return View(obj);
        //    }


        //}



        public IActionResult Delete(int? key)
        {

            if (key == null || key == 0)
            {
                return NotFound();
            }

            var obj = _db.FirstOrDefault(u=>u.Id==key, includeProperties:"Category,ApplicationThype");
            

            if (obj == null)
            {
                return NotFound();
            }
            // pass this specified object to View
            return View(obj);


        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product obj)
        {
            // Delete image file from the Server

            var ImageName = _db.FirstOrDefault(u => u.Id == obj.Id,isTracking:false).Image;
            

            string webRootPath = _webHostEnvironment.WebRootPath;   // IWebHostEnvironment is injected by the 
            string upload = webRootPath + WC.ImagePath;


            // Delete old file
            var fileToBeDeleted = Path.Combine(upload, ImageName);
            if (System.IO.File.Exists(fileToBeDeleted))
            {
                System.IO.File.Delete(fileToBeDeleted);
            }
            else
            {
                return NotFound();
            }



            // Delete the Product


            _db.Remove(obj);
            _db.Save();
                
            return RedirectToAction("index");


        }
    }

  
}
