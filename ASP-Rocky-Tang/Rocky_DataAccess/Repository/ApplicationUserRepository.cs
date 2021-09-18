﻿using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
           _db=db;
        }

        public void Update(ApplicationUser obj)
        {
            //var objFromDb = _db.Category.FirstOrDefault(u => u.Id == obj.Id);
            //var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);     // use base's method
            //if (objFromDb != null)
            //{
            //    objFromDb.Name = obj.Name;
            //    objFromDb.DisplayOrder = obj.DisplayOrder;
            //}
            _db.ApplicationUser.Update(obj);
        }
    }
}
