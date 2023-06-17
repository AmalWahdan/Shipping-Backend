using Shipping.BLL.Dtos;
using Shipping.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.BLL
{
    public interface IOrderManager
    {
        Task<bool> Add(AddOrderDto order);
        Task<bool> Update(UpdateOrderDto order);
        bool Delete(int order);
        IEnumerable<ReadOrderReportsDto> GetAll(int pageNumer, int pageSize);
        IEnumerable<ReadOrderReportsDto> SearchByDateAndStatus(int pageNumer, int pageSize, DateTime fromDate, DateTime toDate, OrderStatus status);
        int CountAll();
        int CountOrdersByDateAndStatus(DateTime fromDate, DateTime toDate, OrderStatus status);
        UpdateOrderDto GetById(int orderId);
        IEnumerable<ReadOrderDto> GetAllByStatus(OrderStatus orderStatus);
        List<int> CountOrdersForEmployeeByStatus();
        List<int> CountOrdersForMerchantByStatus(string merchantId);

        IEnumerable<ReadOrderDto> GetOrdersForEmployee(string searchText,int statusId, int pageNumer, int pageSize);
        IEnumerable<ReadOrderDto> GetOrdersForMerchant(string searchText, string merchantId, int statusId, int pageNumer, int pageSize);

        int GetCountOrdersForEmployee(int statusId,string searchText);
        int GetCountOrdersForMerchant(string merchantId, int statusId, string searchText);
    }
}
