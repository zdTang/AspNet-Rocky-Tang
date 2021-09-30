using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;

namespace Rocky_DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
           _db=db;
        }

 

        public void Update(OrderDetail obj)
        {
            //var objFromDb = _db.Category.FirstOrDefault(u => u.Id == obj.Id);
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);     // use base's method
            if (objFromDb != null)
            {
                _db.OrderDetail.Update(obj);   // All properties will be updated 
            }
        }
    }
}
