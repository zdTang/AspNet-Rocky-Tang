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
        //private readonly ApplicationDbContext _db;
        private readonly ICategoryRepository _cr;
        private readonly IProductRepository _db;
        
        //[BindProperty]
        //private DetailsVM detailsVM { set; get; }

        /* private readonly IMyDependency _myDependency; */    // test DI

        public HomeController(
            ILogger<HomeController> logger,
            /*IMyDependency myDependency,*/
            //ApplicationDbContext db
            IProductRepository db,
            ICategoryRepository cr
            )
        {
            _logger = logger;
            //_myDependency = myDependency;
            _db = db;
            _cr = cr;
#if DEBUG
            _logger.LogWarning("instantiate-- HomeController");

#endif

        }
    

        public IActionResult Index()
        {
#if DEBUG
            _logger.LogWarning("Home  Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif



            //  Testing DI
            //_myDependency.WriteMessage("TESTING Dependency Injection !!!");

            //  Test get TYPE information of Class Category

            //Type categoryType = typeof(Category);
            //PropertyInfo[] propertyList = categoryType.GetProperties();
            HomeVM homeVM = new HomeVM()
            {
                //Category, ApplicationType will be used at _individualProductCard  partial Table
                Products = _db.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _cr.GetAll()
            };
#if DEBUG
            _logger.LogWarning(" ==> view:Home/Index(homeVM)");
#endif

            return View(homeVM);

        }


        public IActionResult Details(int Id)
        {
        #if DEBUG
            _logger.LogWarning("Home  Controller--Details");
            _logger.LogWarning(User?.Identity?.Name);
        #endif 
            var mySession = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            var detailsVM = new DetailsVM()
           
            {
                //Product = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType).Where(u => u.Id == Id).FirstOrDefault(),
                Product = _db.FirstOrDefault(u => u.Id == Id,includeProperties:"Category,ApplicationType"),

                ExistsInCart = (mySession != null && mySession.Exists(u => u.ProductId == Id)) ? true : false
            };
#if DEBUG
            _logger.LogWarning(" ==> view:Home/Detail(detailsVM)");
#endif

            return View(detailsVM);
        }


        //  DetailsVM dm is not declared as Bind peroperty
        //  SO We need use it explicately !!!
        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int Id, DetailsVM dm)
        {
#if DEBUG
            _logger.LogWarning("Home  Controller--Details--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif

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

            shoppoingCartList.Add(new ShoppingCart { ProductId = Id, SqFt= dm.Product.TempSqFt });
            HttpContext.Session.Set(WC.SessionCart, shoppoingCartList);
            TempData[WC.Success] = "Add to Cart Successful!";
#if DEBUG
            _logger.LogWarning(" R==> Action:Home/Index");
#endif

            return RedirectToAction(nameof(Index));
        }


        public IActionResult RemoveFromCart(int Id)
        {
#if DEBUG
            _logger.LogWarning("Home  Controller--RemoveFromCart");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            var shoppoingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

         if(shoppoingCartList != null && shoppoingCartList.Count() > 0)
            {
                shoppoingCartList.Remove(shoppoingCartList.Find(item => item.ProductId == Id));
            }

            
            HttpContext.Session.Set(WC.SessionCart, shoppoingCartList);
            TempData[WC.Success] = "Successful!";
#if DEBUG
            _logger.LogWarning(" R==> Action:Home/Index");
#endif

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
