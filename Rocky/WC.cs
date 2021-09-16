﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// This class is used as a globally container of variables
/// </summary>
namespace Rocky
{
    // To get the complete path of this image\product folder, we still need the environment paht
    // which we can get from IWebHostEnvironment (whose implement has been injected with DI)
    public static class WC
    {
       
        public static string ImagePath = @"\images\product\";     /**This folder is for containing images*/
        public static string SessionCart = "ShoppingCartSession";


        // Add two strings to represent roles

        public static string AdminRole = "Admin";
        public static string CustomerRole = "Customer";

        // About Email

        public static string EmailSender = "miketangtest@gmail.com";
        public static string EmailSenderPass = "Temp123456*";

        public static string EmailRecevier = "michael.tang.ca@gmail.com";
    }
}
