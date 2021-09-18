﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ApplicationController(IApplicationTypeRepository db)
        {
            _db = db;                                          //  Dependency Injection
        }
        
        public IActionResult Index()
        {

            IEnumerable<ApplicationType> objList = _db.GetAll();     //  Grab a collection from DB
            


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
        public IActionResult Create(ApplicationType obj)
        {
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

            //var obj = _db.ApplicationType.Find(key);
            var obj = _db.Find(key.GetValueOrDefault());
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
        public IActionResult Edit(ApplicationType obj)
        {
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

            //var obj = _db.ApplicationType.Find(key);

            var obj = _db.Find(key.GetValueOrDefault());

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
        public IActionResult Delete(ApplicationType obj)
        {

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
            return RedirectToAction("index");


        }
    }

  
}
