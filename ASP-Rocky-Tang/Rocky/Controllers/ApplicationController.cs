using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class ApplicationController : Controller
    {

        //private readonly ApplicationDbContext _db;
        private readonly IApplicationTypeRepository _db;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApplicationTypeRepository db, ILogger<ApplicationController> logger) {
            _db = db;
            _logger=logger;
            //  Dependency Injection
#if DEBUG
            _logger.LogWarning("instantiate-- Application  Controller");
#endif

        }
    
        
        public IActionResult Index()
        {
#if DEBUG
            _logger.LogWarning("iApplication  Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            IEnumerable<ApplicationType> objList = _db.GetAll();     //  Grab a collection from DB

#if DEBUG
            _logger.LogWarning(" ==> view:Application/Index");
#endif

            return View(objList);
            //var content = new ContentResult();
            //content.Content = "hello";
            //return content;
        }

        // Create a new Category
        public IActionResult Create()
        {
#if DEBUG
            _logger.LogWarning("iApplication  Controller--Create");
            _logger.LogWarning(User?.Identity?.Name);
            _logger.LogWarning(" ==> view:Application/Create");
#endif



            return View();

        }
        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
#if DEBUG
            _logger.LogWarning("Application  Controller--Create--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            if (ModelState.IsValid)
            {
                //_db.ApplicationType.Add(obj);
                //_db.SaveChanges();

                _db.Add(obj);
                _db.Save();
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
                TempData[WC.Success] = "Application Type created successfully!";
#if DEBUG
                _logger.LogWarning(" R==> view:Application/Index");
#endif

                return RedirectToAction("index");
            }
            else
            {
                // The validation is server-side validation
                // Those Error information will display only we respond this view
                // Those Errow information are not been activated when input so that they are not Client-side validation
                TempData[WC.Error] = "Application type has not been created!";
#if DEBUG
                _logger.LogWarning(" ==> view:Application/Create");
#endif

                return View(obj);
            }
            

        }


        // Create a new Category
        public IActionResult Edit(int? key)
        {
#if DEBUG
            _logger.LogWarning("Application  Controller--EDIT");
            _logger.LogWarning(User?.Identity?.Name);
#endif



            if (key==null||key==0)
            {
                return NotFound();
            }

            //var obj = _db.ApplicationType.Find(key);
            var obj = _db.Find(key.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            // pass this specified object to View
            // TempData[WC.Success] = "Application Type has been edited successfully!";
#if DEBUG
            _logger.LogWarning(" ==> view:Application/Edit");
#endif

            return View(obj);

        }


        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
#if DEBUG
            _logger.LogWarning("Application  Controller--EDIT--post");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            if (ModelState.IsValid)
            {
                //_db.ApplicationType.Update(obj);
                //_db.SaveChanges();

                _db.Update(obj);
                _db.Save();
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
                TempData[WC.Success] = "Application Type has been edited successfully!";
#if DEBUG
                _logger.LogWarning(" R==> Action:Application/Index");
#endif

                return RedirectToAction("index");
            }
            else
            {
                // The validation is server-side validation
                // Those Error information will display only we respond this view
                // Those Errow information are not been activated when input so that they are not Client-side validation
                TempData[WC.Error] = "Application Type has not been edited!";
#if DEBUG
                _logger.LogWarning(" ==> view:Application/Edit");
#endif

                return View(obj);
            }


        }


        // Create a new Category
        public IActionResult Delete(int? key)
        {
#if DEBUG
            _logger.LogWarning("Application  Controller--Delete");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            if (key == null || key == 0)
            {
                return NotFound();
            }

            //var obj = _db.ApplicationType.Find(key);

            var obj = _db.Find(key.GetValueOrDefault());

            if (obj == null)
            {
                return NotFound();
            }
            // pass this specified object to View
#if DEBUG
            _logger.LogWarning(" ==> view:Application/Delete");
#endif

            return View(obj);


        }

        // Create a new Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(ApplicationType obj)
        {
#if DEBUG
            _logger.LogWarning("Application  Controller--Delete--post");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            //_db.ApplicationType.Remove(obj);
            //_db.SaveChanges();

            _db.Remove(obj);
            _db.Save();
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
            TempData[WC.Success] = "Application Type has been removed successfully!";
#if DEBUG
            _logger.LogWarning(" R==> Action:Application/Index");
#endif

            return RedirectToAction("index");


        }
    }

  
}
