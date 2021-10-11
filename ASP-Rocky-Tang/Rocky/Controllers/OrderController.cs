using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
using System.Linq;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]  //  this attribute will prevent unregistered user to access this controller
    //[Authorize]                    //   must login, but not to be necessarily a Admin
    public class OrderController : Controller
    {

        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;
        private readonly ILogger<CartController> _logger;
        private readonly IBrainTreeGate _brain;


        //[BindProperty]

        //public OrderVM OrderVM { get; set; }

        public OrderController(

            IOrderHeaderRepository orderHeaderRepo,
            IOrderDetailRepository orderDetailRepo, 
            ILogger<CartController> logger,
            IBrainTreeGate brain)
        {
            _orderHeaderRepo = orderHeaderRepo;
            _orderDetailRepo = orderDetailRepo;
            _logger = logger;
            _brain = brain;            //  BrainTree
#if DEBUG   
            _logger.LogWarning("instantiate-- Cart Controller");

#endif
        }
        /// <summary>
        /// Retrive Order from Db and display status
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
#if DEBUG
            _logger.LogWarning("Order Controller--Index");
            _logger.LogWarning(User?.Identity?.Name);
#endif

            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHeaderList=_orderHeaderRepo.GetAll(),
                StatusList=WC.listStatus.ToList().Select(i=>new SelectListItem
                {
                    Text=i,
                    Value=i
                })
                //status=WC.listStatus
            };


            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }

#if DEBUG
            _logger.LogWarning("Controllor:Order/Index ==> /order/index view");
#endif

                return View(orderListVM);

        }


        
    }
}
