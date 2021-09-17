using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Models.ViewModels
{
    public class DetailsVM
    {
        public Product Product { set; get; }
        public bool ExistsInCart { set; get; }
        //public DetailsVM()
        //{
        //    Product = new Product();
        //}
    }
}
