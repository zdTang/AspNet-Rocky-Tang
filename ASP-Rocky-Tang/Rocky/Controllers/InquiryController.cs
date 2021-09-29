using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class InquiryController : Controller
    {


        private readonly ILogger<ApplicationController> _logger;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;

        [BindProperty]     // Bind it with Actions
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(
            ILogger<ApplicationController> logger,
            IInquiryDetailRepository inquiryDetailRepo,
            IInquiryHeaderRepository inquiryHeaderRepo
            )
            {
                _logger=logger;
                _inquiryDetailRepo = inquiryDetailRepo;
                _inquiryHeaderRepo = inquiryHeaderRepo;
#if DEBUG
            _logger.LogWarning("instantiate-- Inquiry  Controller");
#endif

        }

        public IActionResult Index()
        {
#if DEBUG
            _logger.LogWarning("Inquiry  Controller-- Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif
            _logger.LogWarning(" ==> view:Inquiry/Index");
            return View();
        }


        public IActionResult Details(int id)
        {
        #if DEBUG
            _logger.LogWarning("Inquiry  Controller-- Detail");
            _logger.LogWarning(User?.Identity?.Name);
        #endif

            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepo.FirstOrDefault(u=>u.Id==id),
                InquiryDetails = _inquiryDetailRepo.GetAll(u => u.InquiryHeaderId == id,includeProperties:"Product")
            };
#if DEBUG
            _logger.LogWarning(" ==> view:Inquiry/Details(InquiryVM)");
#endif

            return View(InquiryVM);
        }

        //InquiryVM is binded with this action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
        #if DEBUG
            _logger.LogWarning("Inquiry  Controller--Detail--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            InquiryVM.InquiryDetails = _inquiryDetailRepo.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
            

            foreach(var detail in InquiryVM.InquiryDetails)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);
#if DEBUG
            _logger.LogWarning(" R ==> Action:Cart/Index");
#endif

            return RedirectToAction("index", "Cart");
        }


        //InquiryVM is binded with this action
        [HttpPost]

        public IActionResult Delete()
        {
#if DEBUG
            _logger.LogWarning("Inquiry  Controller--Delete--Post");
            _logger.LogWarning(User?.Identity?.Name);
#endif
            InquiryHeader inquiryHeader = _inquiryHeaderRepo.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inquiryDetailRepo.GetAll(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
            
            
            _inquiryDetailRepo.RemoveRange(inquiryDetails);     //  Need use Loop to remove all inquiry detail
            _inquiryHeaderRepo.Remove(inquiryHeader);

            _inquiryHeaderRepo.Save();

            TempData[WC.Success] = "Successfully!";
#if DEBUG
            _logger.LogWarning("  R ==> Action:Inquiry/index");
#endif


            return RedirectToAction(nameof(Index));
        }






        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepo.GetAll() });
        }
        #endregion

    }


}
