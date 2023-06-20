using Shipping.BLL.Dtos;
using Shipping.BLL.Dtos.RepresentativeDtos;
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
        Task<AddOrderResultDto> Add(AddOrderDto order);
        Task<UpdateOrderResultDto> Update(UpdateOrderDto order);
        bool Delete(int order);
        IEnumerable<ReadOrderReportsDto> GetAll(int pageNumer, int pageSize);
        IEnumerable<ReadOrderReportsDto> SearchByDateAndStatus(int pageNumer, int pageSize, DateTime fromDate, DateTime toDate, OrderStatus status);
        int CountAll();
        int CountOrdersByDateAndStatus(DateTime fromDate, DateTime toDate, OrderStatus status);
        UpdateOrderDto GetById(int orderId);
        List<int> CountOrdersForEmployeeByStatus();
        List<int> CountOrdersForMerchantByStatus(string merchantId);
        List<int> CountOrdersForRepresentativeByStatus(string representativeId);

        IEnumerable<ReadOrderDto> GetOrdersForEmployee(string searchText,int statusId, int pageNumer, int pageSize);
        IEnumerable<ReadOrderDto> GetOrdersForMerchant(string searchText, string merchantId, int statusId, int pageNumer, int pageSize);

        int GetCountOrdersForEmployee(int statusId,string searchText);
        int GetCountOrdersForMerchant(string merchantId, int statusId, string searchText);


        bool ChangeStatus(int OrderId, OrderStatus status);
        bool SelectRepresentative(int OrderId, string representativeId);

        int GetCountOrdersForRepresentative(string representativeId, int statusId, string searchText);
        IEnumerable<ReadOrderDto> GetOrdersForRepresentative(string representativeId, int statusId, int pageNumer, int pageSize, string searchText);
        Task<List<DropdownListRepresentativeDto>> DropdownListRepresentativeAsync(int orderId);

        ReadAllOrderDataDto GetAllDataById(int orderId);

        bool ChangeStatusAndReasonRefusal(int OrderId, OrderStatus status, int? reasonRefusal);
    }
}
