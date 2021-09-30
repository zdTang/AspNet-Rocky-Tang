﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        //private readonly ApplicationDbContext _db;
        private readonly ICategoryRepository _cr;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(
            ICategoryRepository cr,
            ILogger<CategoryController> logger
             )
        {
            //_db = db;  //  Dependency Injection
            _cr = cr;
            _logger = logger;
#if DEBUG
            _logger.LogWarning("instantiate-- Category Controller");

#endif

        }
    
        
        public IActionResult Index()
        {
#if DEBUG
            _logger.LogWarning("Category Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            //IEnumerable<Category> objList = _db.Category;     //  Grab a collection from DB
            IEnumerable<Category> objList = _cr.GetAll();     //  Grab a collection from DB

            return View(objList);
            //var content = new ContentResult();
            //content.Content = "hello";
            //return content;
        }

        // Create a new Category
        public IActionResult Create()
        {
#if DEBUG
            _logger.LogWarning("Category Controller--create");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            return View();

        }
        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
#if DEBUG
            _logger.LogWarning("Category Controller--create--post");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            if (ModelState.IsValid)
            {
                //_db.Category.Add(obj);
                _cr.Add(obj);
               // _db.SaveChanges();
                _cr.Save();
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
                TempData[WC.Success] = "Category created successfully!";
                return RedirectToAction("index");
            }
            else
            {
                // The validation is server-side validation
                // Those Error information will display only we respond this view
                // Those Errow information are not been activated when input so that they are not Client-side validation
                TempData[WC.Error] = "Error while create category!";
                return View(obj);
            }
            

        }


        // Create a new Category
        public IActionResult Edit(int? key)
        {
#if DEBUG
            _logger.LogWarning("Category Controller--Edit");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            if (key==null||key==0)
            {
                return NotFound();
            }

            //var obj = _db.Category.Find(key);
            var obj = _cr.Find(key.GetValueOrDefault());
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
#if DEBUG
            _logger.LogWarning("Category Controller--Edit--post");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            if (ModelState.IsValid)
            {
                //_db.Category.Update(obj);
                _cr.Update(obj);
                //_db.SaveChanges();
                _cr.Save();
                TempData[WC.Success] = "Category edited successfully!";
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
                TempData[WC.Error] = "Error while edit category!";
                return View(obj);
            }


        }


        // Create a new Category
        public IActionResult Delete(int? key)
        {
#if DEBUG
            _logger.LogWarning("Category Controller--delete");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            if (key == null || key == 0)
            {
                return NotFound();
            }

            //var obj = _db.Category.Find(key)
              var obj = _cr.Find(key.GetValueOrDefault());
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
#if DEBUG
            _logger.LogWarning("Category Controller--delete --post");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            // _db.Category.Remove(obj);
            // _db.SaveChanges();

            _cr.Remove(obj);
            _cr.Save();

            TempData[WC.Success] = "Category deleted successfully!";
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
