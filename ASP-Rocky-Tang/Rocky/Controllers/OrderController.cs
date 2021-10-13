using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
using System;
using System.Linq;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]  //  this attribute will prevent unregistered user to access this controller
    //[Authorize]                    //   must login, but not to be necessarily a Admin
    public class OrderController : Controller
    {
        [BindProperty]
       public OrderVM orderVM { set; get; }
        
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
        /// Retrive Orders Headers  from Db and list as table
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



        /// <summary>
        /// Load order details based on coming order header ID
        /// One order headerID may contains several orderDetails
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            _logger.LogWarning("Order Controller--Details");
            _logger.LogWarning(User?.Identity?.Name);
            orderVM = new OrderVM {
                /*=====================
                 
                Find is only implemented in List<T>, 
                while Where().FirstOrDefault() works with all IEnumerable<T>.
                 
                 ======================*/
                orderHeader = _orderHeaderRepo.Find(id),                       // Try this, it works
                //orderHeader = _orderHeaderRepo.FirstOrDefault(u=>u.Id==id),  // WORKS
                orderDetails = _orderDetailRepo.GetAll(item => item.OrderHeaderId == id,includeProperties:"Product")
            };

            return View(orderVM);
        }
        /// <summary>
        ///  Data will come from order-detail view
        ///  OrderVM is the binding view model
        /// </summary>
        /// <returns></returns>


        [HttpPost]
        public IActionResult StartProcessing()
        {
            _logger.LogWarning("Order Controller--StartProcessing");
            _logger.LogWarning(User?.Identity?.Name);

            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == orderVM.orderHeader.Id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderHeaderRepo.Save();
            TempData[WC.Success] = "Order Processing Successfully!!";
            return RedirectToAction(nameof(Index));
        }




        /// <summary>
        /// This action will update order status to shipped 
        /// and update shipping date too
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public IActionResult ShipOrder()
        {
            _logger.LogWarning("Order Controller--ShipOrder");
            _logger.LogWarning(User?.Identity?.Name);

            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == orderVM.orderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHeaderRepo.Save();
            TempData[WC.Success] = "Order Shipped Successfully!!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult CancelOrder()
        {
            _logger.LogWarning("Order Controller--CancelOrder");
            _logger.LogWarning(User?.Identity?.Name);

            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(u => u.Id == orderVM.orderHeader.Id);

            var gateway = _brain.CreateGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if(transaction.Status==TransactionStatus.AUTHORIZED|| transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //NO REFUND
                Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                // Refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }

            orderHeader.OrderStatus = WC.StatusRefunded;
            _orderHeaderRepo.Save();
            TempData[WC.Success] = "Order Cancelled Successfully!!";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult UpdateOrderDetails(/*OrderHeader o*/)
        {
            _logger.LogWarning("Order Controller--UpdateOrderDetails");
            _logger.LogWarning(User?.Identity?.Name);
            // We uill not update all fields with update(), that's not a good approach !
            //_orderHeaderRepo.Update(o);
            OrderHeader orderHeaderFromDB = _orderHeaderRepo.FirstOrDefault(u => u.Id == orderVM.orderHeader.Id);

            orderHeaderFromDB.FullName = orderVM.orderHeader.FullName;
            orderHeaderFromDB.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHeaderFromDB.City = orderVM.orderHeader.City;
            orderHeaderFromDB.State = orderVM.orderHeader.State;
            orderHeaderFromDB.PostalCode = orderVM.orderHeader.PostalCode;
            orderHeaderFromDB.Email = orderVM.orderHeader.Email;

            _orderHeaderRepo.Save();
            TempData[WC.Success] = "Order Details Updated Successfully!!";
            // be aware the third parameter must be object
            return RedirectToAction("Details","Order",new {id=orderHeaderFromDB.Id});
        }

    }


}

