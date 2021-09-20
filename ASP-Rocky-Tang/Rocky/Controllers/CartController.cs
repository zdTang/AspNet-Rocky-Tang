using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
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
        //private readonly ApplicationDbContext _db;
        private readonly IProductRepository _productRepo;
        private readonly IApplicationUserRepository _applicationUserRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;
        private readonly IWebHostEnvironment _en;
        private readonly IEmailSender _es;
        private readonly ILogger<CartController> _logger;


        [BindProperty]
        public ProductUserVM productUserVM { set; get; }

        public CartController(
            IProductRepository productRepo,
            IApplicationUserRepository applicationUserRepo,
            IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailRepository inquiryDetailRepo,
            IWebHostEnvironment en, 
            IEmailSender es,
            ILogger<CartController> logger)
        {
            _productRepo = productRepo;
            _applicationUserRepo = applicationUserRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;
            _en = en;
            _es = es;
            _logger = logger;

            _logger.LogWarning("instantiate-- Cart Controller");
            _logger.LogWarning(User?.Identity?.Name);
        }
        
        public IActionResult Index()
        {
            _logger.LogWarning("Cart Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
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
                IEnumerable<Product> prodList = _productRepo.GetAll(u => prodInCart.Contains(u.Id));
                return View(prodList);
            }

            return View("home/index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            _logger.LogWarning("Cart Controller--Index--Post");
            _logger.LogWarning(User?.Identity?.Name);
            return RedirectToAction(nameof(Summary));
        }



        public IActionResult Summary()
        {
            // Dealing with Identity
            _logger.LogWarning("Cart Controller--Summary");
            _logger.LogWarning(User?.Identity?.Name);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.Name);

            // Dealing with shopping cart

            var cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            if (cartItems != null && cartItems.Count() > 0)
            {
                List<int> prodInCart = cartItems.Select(i => i.ProductId).ToList();           //  get product ID LIST
    
                productUserVM = new ProductUserVM()
                {
                    ApplicationUser = _applicationUserRepo.FirstOrDefault(u => u.Id == claim.Value),
                    ProductList= _productRepo.GetAll(u => prodInCart.Contains(u.Id)).ToList()
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
        #if DEBUG
            _logger.LogWarning("Cart Controller--Summary--Post");
            _logger.LogWarning(User?.Identity?.Name);
        #endif

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

        #if DEBUG
            _logger.LogInformation("claim==" + claim.Type);
            _logger.LogInformation("claim==" + claim.Value);
        #endif

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

            // Add Inquiry stuff

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };

            _inquiryHeaderRepo.Add(inquiryHeader);   // save to Database
            _inquiryHeaderRepo.Save();

            foreach(var prod in productUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,     // can this get the ID
                    ProductId = prod.Id
                };
                _inquiryDetailRepo.Add(inquiryDetail);   // save to Database
          
            }

            _inquiryDetailRepo.Save();




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
                IEnumerable<Product> prodList = _productRepo.GetAll(u => prodInCart.Contains(u.Id));
                return View(nameof(Index),prodList);
            }

            return View("home/index");
        }
    }
}
