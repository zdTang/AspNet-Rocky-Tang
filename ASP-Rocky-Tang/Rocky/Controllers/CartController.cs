using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
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
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;
        private readonly IWebHostEnvironment _en;
        private readonly IEmailSender _es;
        private readonly ILogger<CartController> _logger;
        private readonly IBrainTreeGate _brain;


        [BindProperty]
        public ProductUserVM productUserVM { set; get; }

        public CartController(
            IProductRepository productRepo,
            IApplicationUserRepository applicationUserRepo,
            IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailRepository inquiryDetailRepo,
            IOrderHeaderRepository orderHeaderRepo,
            IOrderDetailRepository orderDetailRepo,
            IWebHostEnvironment en, 
            IEmailSender es,
            ILogger<CartController> logger,
            IBrainTreeGate brain)
        {
            _productRepo = productRepo;
            _applicationUserRepo = applicationUserRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
            _inquiryDetailRepo = inquiryDetailRepo;
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
            _en = en;
            _es = es;
            _logger = logger;
            _brain = brain;            //  BrainTree
#if DEBUG   
            _logger.LogWarning("instantiate-- Cart Controller");

#endif

        }
        
        public IActionResult Index()
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            /*
             Session bind with Request from a browser client.
             Even the client has not register,
             the Session can save information
              
             */


            //List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();             // create a empty item List
            var shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
           
            if(shoppingCartList != null&&shoppingCartList.Count()>0)
            {



               // I don't follow his code, just use mine

                IList<Product> prodList = new List<Product>();

                foreach (var cartObj in shoppingCartList)
                {
                    Product prodTemp = _productRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                    prodTemp.TempSqFt = cartObj.SqFt;
                    prodList.Add(prodTemp);
                }


#if DEBUG
                _logger.LogWarning("Controllor:Cart  Action: Index ==> /cart/index view");
#endif



                return View(prodList);
            }
#if DEBUG
            _logger.LogWarning("Controllor:Cart  Action: Index ==> home/index view");
#endif

            return View("home/index");
        }


        /// <summary>
        /// Once use click "Continue"
        /// At this moment, Admin could update the quantity
        /// Here to push the Quantity into the Session as well
        /// </summary>
        /// <param name="ProdList"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IList<Product> ProdList)
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--Index--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif




            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, SqFt = prod.TempSqFt });
            }

            // Before, the sessionCart has only ID, Now, it has been updated with the ID and Quantity, which was updated by Admin
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

#if DEBUG
            _logger.LogWarning(" R==> Action: Cart/Summary ");
#endif


            return RedirectToAction(nameof(Summary));
        }


        /// <summary>
        /// User click "Update Cart"
        /// Update Session CartList with new item Quantity
        /// The Inquiry ID should be in the Session
        /// </summary>
        /// <param name="ProdList"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult UpdateCart(IList<Product> ProdList)
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--UpdateCart--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif


            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach(Product prod in ProdList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, SqFt = prod.TempSqFt });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
#if DEBUG
            _logger.LogWarning(" R==> Action: Cart/Index ");
#endif

            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Once use click "Continue" 
        /// Redirect from IndexPost to here
        /// </summary>
        /// <returns></returns>

        public IActionResult Summary()
        {

            /*
             Three scenarios here !
             1. user login and deal with their order
            2. admin login and deal with order for customer visit shop(need show blank for input info by the admin)
            3. admin login and deal with inquiry order (need display customer information)
             
             */





            // Dealing with Identity
#if DEBUG
            _logger.LogWarning("Cart Controller--Summary");
            _logger.LogWarning(User?.Identity?.Name);
#endif




            ApplicationUser applicationUser;

            // Here, the Administrator will manage the order, so the customer's name need to be populated 
            // other than the Administrator's own information

            if (User.IsInRole(WC.AdminRole))
            {
                var _IdInSession = HttpContext.Session.Get<int>(WC.SessionInquiryId);
                if (_IdInSession != 0)
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _inquiryHeaderRepo.FirstOrDefault(u => u.Id == _IdInSession);

                    applicationUser = new ApplicationUser { 
                    Email=inquiryHeader.Email,
                    FullName=inquiryHeader.FullName,
                    PhoneNumber=inquiryHeader.PhoneNumber
                    };
                }
                else   // Manager place order for user who visit the shop
                {
                    applicationUser = new ApplicationUser();
                }

                // use BrainTree gate way
                var gateway = _brain.GetGateway();                    // Instantiate an instance of BraintreeGateway
                var clientToken = gateway.ClientToken.Generate();     // Use BraintreeGateway Instance to generate a clientToken
                ViewBag.ClientToken = clientToken;                    // This token will send to frontEnd 
                // use this to 
                ViewBag.UserCategory = WC.AdminRole;               //  tell front end to execute braintree script code
            }

            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirstValue(ClaimTypes.Name);
                applicationUser = _applicationUserRepo.FirstOrDefault(u => u.Id == claim.Value);
                
            }

            
            
            // Dealing with shopping cart

            var cartItems = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            if (cartItems != null && cartItems.Count() > 0)
            {
                List<int> prodInCart = cartItems.Select(i => i.ProductId).ToList();           //  get product ID LIST
    
                productUserVM = new ProductUserVM()
                {
                    //ApplicationUser = _applicationUserRepo.FirstOrDefault(u => u.Id == claim.Value),
                    ApplicationUser = applicationUser,
                    /*This productList locks information of SqFt*/
                    //ProductList = _productRepo.GetAll(u => prodInCart.Contains(u.Id)).ToList()
                };
                //  Here to add SqFt Information which is in the Session
                foreach(var cartObj in cartItems)
                {
                    Product productTemp = _productRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                    productTemp.TempSqFt = cartObj.SqFt;
                    productUserVM.ProductList.Add(productTemp);
                }

#if DEBUG
                _logger.LogWarning("==> View: Cart/Summary ");
#endif

                return View(productUserVM);
            }

            else  // is this possible?
            {
#if DEBUG
                _logger.LogWarning("==> View: Cart/Index ");
#endif

                return View("home/index");
               
            }

            
        }
        /// <summary>
        ///  Once User click " Submit Order "
        ///  If user is admin, then he will push the order into Database
        ///  If user is normal user, he will send a Email to the Store
        /// </summary>
        /// <param name="productUserVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]

        public async Task<IActionResult> SummaryPost(IFormCollection formCollection, ProductUserVM productUserVM)
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


            if (User.IsInRole(WC.AdminRole))
            {
                // send order to Database
                var orderTotal = 0.0;
                foreach(Product prod in productUserVM.ProductList)
                {
                    orderTotal += (prod.TempSqFt)*(prod.Price);
                }

                // Order Header

                OrderHeader orderHeader = new OrderHeader
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = orderTotal,
                    City = productUserVM.ApplicationUser.City,
                    StreetAddress = productUserVM.ApplicationUser.StreetAddress,
                    State = productUserVM.ApplicationUser.State,
                    PostalCode = productUserVM.ApplicationUser.PostalCode,
                    FullName = productUserVM.ApplicationUser.FullName,
                    Email = productUserVM.ApplicationUser.Email,
                    PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };

                _orderHeaderRepo.Add(orderHeader);
                _orderHeaderRepo.Save();

                //  order Detail

                foreach (var prod in productUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId= orderHeader.Id,  //  we don't need care about this ID
                        PricePerSqFt=prod.Price,
                        Sqft=prod.TempSqFt,
                        ProductId=prod.Id
                    };
                    _orderDetailRepo.Add(orderDetail);   // save to Database
                }
                _orderDetailRepo.Save();


#if DEBUG
                _logger.LogWarning(" Push order to DB, then  R==> Action: Cart/InquiryConfirmation");
#endif

                // Need an action to delete or mark those inquiries which have been processed !!!

                #region Delete inquiryID which has been processed

                var _IdInSession = HttpContext.Session.Get<int>(WC.SessionInquiryId);
                if (_IdInSession != 0)
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _inquiryHeaderRepo.FirstOrDefault(u => u.Id == _IdInSession);


                    IEnumerable<InquiryDetail> inquiryDetails = _inquiryDetailRepo.GetAll(u => u.InquiryHeaderId == _IdInSession);


                    _inquiryDetailRepo.RemoveRange(inquiryDetails);     //  Need use Loop to remove all inquiry detail
                    _inquiryHeaderRepo.Remove(inquiryHeader);

                    _inquiryHeaderRepo.Save();

                    TempData[WC.Success] = $"Inquiry No.{_IdInSession} has been processed successfully! and remove it now!";

                }

                #endregion

                #region  Deal with BrainTree

                //payment_method_nonce

                string nonceFromTheClient = formCollection["payment_method_nonce"];
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),    // The money we want to charge  !!! should be very accurate
                    PaymentMethodNonce = nonceFromTheClient,  // this Nounce is passed from Braintree
                    //DeviceData = deviceDataFromTheClient,   // we don't need this data
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brain.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);// Sent Request by Gateway

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }
                #endregion

                //  Never forget save all update !
                _inquiryHeaderRepo.Save();

                return RedirectToAction(nameof(InquiryConfirmation),new {id=orderHeader.Id}); //??

            }
            else
            {
                // create an Inquiry
                // Dealing with Identity
                var PathToTemplate = _en.WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";

                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                StringBuilder productListSB = new StringBuilder();
                foreach (var prod in productUserVM.ProductList)
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

                foreach (var prod in productUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,     // can this get the ID
                        ProductId = prod.Id
                    };
                    _inquiryDetailRepo.Add(inquiryDetail);   // save to Database

                }

                _inquiryDetailRepo.Save();

#if DEBUG
                _logger.LogWarning("Send Email !  then R==> Action: Cart/InquiryConfirmation");
#endif


                return RedirectToAction(nameof(InquiryConfirmation));

            }
            
            
            
            
            
            
            
            
            
            
            
            
            
        }

        public IActionResult InquiryConfirmation(int id=0)
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--Cart--InquiryConfirmation");
            _logger.LogWarning(User?.Identity?.Name);
#endif
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == id);


            HttpContext.Session.Clear();
#if DEBUG
            _logger.LogWarning("==> view: Cart/InquiryConfirmation");
#endif

            return View(orderHeader);
        }

        public IActionResult Remove(int Id)
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--Cart--Remove");
            _logger.LogWarning(User?.Identity?.Name);
#endif

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
                TempData[WC.Success] = "Item Type has been removed successfully!";
#if DEBUG
                _logger.LogWarning("==> view: Cart/Index");
#endif

                return View(nameof(Index),prodList);
            }
#if DEBUG
            _logger.LogWarning("==> view: home/Index");
#endif

            return View("home/index");
        }



        public IActionResult Clear()
        {
#if DEBUG
            _logger.LogWarning("Cart Controller--Cart/Clear");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            //List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();             // create a empty item List
            HttpContext.Session.Clear();
            
                TempData[WC.Success] = "Cart has been cleared successfully!";
#if DEBUG
                _logger.LogWarning("R==> Action: Home/Index");
#endif

                return RedirectToAction("Index","Home");

        }
    }
}
