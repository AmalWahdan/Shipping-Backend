using Shipping.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.DAL.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll(int pageNumer, int pageSize);
        int CountAll();
        IEnumerable<Order> SearchByDateAndStatus(int pageNumer, int pageSize,DateTime fromDate, DateTime toDate,OrderStatus status);
        int CountOrdersByDateAndStatus(DateTime fromDate, DateTime toDate, OrderStatus status);

        IEnumerable<Order> GetAllByStatus(OrderStatus orderStatus);
        Order GetById(int id);
        bool Add(Order entity);
        bool Delete(Order entity);
        List<int> CountOrdersForEmployeeByStatus();
        List<int> CountOrdersForMerchantByStatus(string merchantId);

        IEnumerable<Order> GetOrdersForEmployee(string searchText, int statusId, int pageNumer, int pageSize);
        IEnumerable<Order> GetOrdersForMerchant(string searchText, string merchantId,int statusId, int pageNumer, int pageSize);
        
        int GetCountOrdersForEmployee(int statusId, string searchText);
        int GetCountOrdersForMerchant(string merchantId, int statusId, string searchText);


        int GetCountOrdersForRepresentative(string representativeId, string searchText);
        IEnumerable<Order> GetOrdersForRepresentative(string representativeId, int pageNumer, int pageSize, string searchText);

        bool SaveChanges();

        
    }
}
