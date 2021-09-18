using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
           _db=db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == WC.CategoryName)
            {
                return _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            if (obj == WC.ApplicationTypeName)
            {
                _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }
            return null;
            
        }

        public void Update(Product obj)
        {
            //var objFromDb = _db.Category.FirstOrDefault(u => u.Id == obj.Id);
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);     // use base's method
            if (objFromDb != null)
            {
                /*======================              
                objFromDb.Name = obj.Name;
                objFromDb.ApplicationId = obj.ApplicationId;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Discription = obj.Discription;
                objFromDb.ShortDesc = obj.ShortDesc;
                objFromDb.Image = obj.Image;
                objFromDb.Price = obj.Price;
                ========================*/

                _db.Product.Update(obj);   // All properties will be updated 

            }
        }
    }
}
