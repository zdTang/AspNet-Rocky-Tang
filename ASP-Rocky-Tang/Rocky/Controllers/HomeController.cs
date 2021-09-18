using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky_Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        //private readonly ICategoryRepository _cr;

        /* private readonly IMyDependency _myDependency; */    // test DI

        public HomeController(
            ILogger<HomeController> logger,
            /*IMyDependency myDependency,*/
            ApplicationDbContext db
            //ICategoryRepository cr
            )
        {
            _logger = logger;
            //_myDependency = myDependency;
            _db = db;
            //_cr = cr;
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


        public IActionResult Details(int Id)
        {

            var mySession = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            var detailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType).Where(u => u.Id == Id).FirstOrDefault(),

                ExistsInCart = (mySession != null && mySession.Exists(u => u.ProductId == Id)) ? true : false
            };


            return View(detailsVM);
        }

        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int Id)
        {
            /*====
            var mySession = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            List<ShoppingCart> shoppoingCartList = new List<ShoppingCart>();
               
            if (mySession != null&& mySession.Count()>0)
            {
                shoppoingCartList = mySession;
            }
            ===============*/
            var mySession = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            List<ShoppingCart> shoppoingCartList = (mySession != null && mySession.Count() > 0)? mySession: new List<ShoppingCart>();

            shoppoingCartList.Add(new ShoppingCart { ProductId = Id });
            HttpContext.Session.Set(WC.SessionCart, shoppoingCartList);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult RemoveFromCart(int Id)
        {
            var shoppoingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

         if(shoppoingCartList != null && shoppoingCartList.Count() > 0)
            {
                shoppoingCartList.Remove(shoppoingCartList.Find(item => item.ProductId == Id));
            }

            
            HttpContext.Session.Set(WC.SessionCart, shoppoingCartList);
            return RedirectToAction(nameof(Index));



        }

        public IActionResult About()
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
