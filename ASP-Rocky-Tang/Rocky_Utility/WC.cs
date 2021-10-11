using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// This class is used as a globally container of variables
/// </summary>
namespace Rocky_Utility
{
    // To get the complete path of this image\product folder, we still need the environment paht
    // which we can get from IWebHostEnvironment (whose implement has been injected with DI)
    public static class WC
    {

        public const string ImagePath = @"\images\product\";     /**This folder is for containing images*/
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquiryIdSession";


        // Add two strings to represent roles

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        // About Email

        public const string EmailSender = "miketangtest@gmail.com";
        public const string EmailSenderPass = "Temp123456*";

        public const string EmailRecevier = "michael.tang.ca@gmail.com";

        public const string CategoryName = "Category";
        public const string ApplicationTypeName = "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";


        public const string StatusPending = "pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "InProcess";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";
        /// <summary>
        ///  Pay attention how to create a string list
        /// </summary>
        public readonly static IEnumerable<string> listStatus = new ReadOnlyCollection<string>(new List<string>
        {
            StatusPending,
            StatusApproved,
            StatusInProcess,
            StatusShipped,
            StatusCancelled,
            StatusRefunded
        });
    }
}
