using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    //[Authorize(Roles = WC.AdminRole)]  //  this attribute will prevent unregistered user to access this controller
    [Authorize]                    //   must login, but not to be necessarily a Admin
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _en;
        private readonly IEmailSender _es;

        [BindProperty]
        public ProductUserVM productUserVM { set; get; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment en, IEmailSender es)
        {
            _db = db;
            _en = en;
            _es = es;
        }
        
        public IActionResult Index()
        {
            /*
             Session bind with Request from a browser client.
             Even the client has not register,
             the Session can save information
              
             */
            
            
            //List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();             // create a empty item List
            var cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
           if(cartItems != null&&cartItems.Count()>0)
            {
                List<int> prodInCart = cartItems.Select(i => i.ProductId).ToList();           //  get product ID LIST
                IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
                return View(prodList);
            }

            return View("home/index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }



        public IActionResult Summary()
        {
            // Dealing with Identity
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.Name);

            // Dealing with shopping cart

            var cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            if (cartItems != null && cartItems.Count() > 0)
            {
                List<int> prodInCart = cartItems.Select(i => i.ProductId).ToList();           //  get product ID LIST
                //IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
                productUserVM = new ProductUserVM()
                {
                    ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                    ProductList= _db.Product.Where(u => prodInCart.Contains(u.Id)).ToList()
                };

                return View(productUserVM);
            }

            else  // is this possible?
            {
                return View("home/index");
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]

        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            // Dealing with Identity
            var PathToTemplate = _en.WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates" + Path.DirectorySeparatorChar.ToString()+"Inquiry.html";




           

            var subject = "New Inquiry";
            string HtmlBody = "";

            using(StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach(var prod in productUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'>(Id:{prod.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                productUserVM.ApplicationUser.FullName,
                productUserVM.ApplicationUser.Email,
                productUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _es.SendEmailAsync(WC.EmailRecevier, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remove(int Id)
        {
            //List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();             // create a empty item List
            var cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            if (cartItems != null && cartItems.Count() > 0)
            {
                cartItems.Remove(cartItems.Find(u => u.ProductId==Id));                 // Remove item
                HttpContext.Session.Set<List<ShoppingCart>>(WC.SessionCart, cartItems); // reset session
                
                // Tutorial's  option is RedirectToAction(nameof(Index))
                // But Redirection is not a good approach so that I just return another view
                List<int> prodInCart = cartItems.Select(i => i.ProductId).ToList();           //  get product ID LIST
                IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
                return View(nameof(Index),prodList);
            }

            return View("home/index");
        }
    }
}
