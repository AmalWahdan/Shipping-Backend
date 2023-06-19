using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL;
using Shipping.BLL.Dtos;
using Shipping.DAL.Data.Models;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrderController(IOrderManager orderManager)
        {
            this._orderManager = orderManager;
        }

        [HttpPost] 
        public async Task<ActionResult> Add(AddOrderDto order)
        {
            var result = await _orderManager.Add(order);
            if (result && ModelState.IsValid)
            {
                return Ok(new { message = "Order was added successfully." });
            }
            ModelState.AddModelError("save", "Can't save Order may be some ID'S wrong!");
            return BadRequest(ModelState);
        }
        
        
        [HttpPut]
        public async Task<ActionResult> Update(UpdateOrderDto order)
        {
            var result =await _orderManager.Update(order);
            if (result && ModelState.IsValid)
            {
                return Ok(new { message = "Order was updated successfully." });
            }
            ModelState.AddModelError("save", "Can't save Order may be some ID'S wrong!");
            return BadRequest(ModelState);
        }

        [HttpDelete]
        public ActionResult Delete(int orderId)
        {
            var result = _orderManager.Delete(orderId);
            if (result)
            {
                return Ok(new { message = "Order was deleted successfully." });
            }
            return BadRequest("Order not found");
        }

        [HttpGet]
        [Route("Get/{id}")]
        public ActionResult<ReadOrderDto> GetById(int id)
        {
            var order = _orderManager.GetById(id);
            if (order != null)
            {
                return Ok(order);
            }
            return BadRequest(new { message = "Item not found" });
        }

        [HttpGet]
        [Route("GetAllOrder")]
        public ActionResult<IEnumerable<ReadOrderReportsDto>> GetAllOrder(int pageNubmer, int pageSize)
        {
            return Ok(_orderManager.GetAll(pageNubmer,pageSize));
        }
        [HttpGet]
        [Route("CountAll")]
        public ActionResult<int> CountAll()
        {
            return Ok(_orderManager.CountAll());
        }
        
        [HttpGet]
        [Route("SearchByDateAndStatus")]
      
        public ActionResult<IEnumerable<ReadOrderReportsDto>> SearchByDateAndStatus(int pageNubmer, int pageSize, DateTime fromDate, DateTime toDate, OrderStatus status)
        {
            return Ok(_orderManager.SearchByDateAndStatus(pageNubmer, pageSize, fromDate, toDate, status));
        }
        [HttpGet]
        [Route("CountOrdersByDateAndStatus")]
        public ActionResult<int> CountOrdersByDateAndStatus(DateTime fromDate, DateTime toDate, OrderStatus status)
        {
            return Ok(_orderManager.CountOrdersByDateAndStatus(fromDate, toDate, status));
        }

        [HttpGet]
        [Route("GetAllByStatus")]
        public ActionResult<IEnumerable<ReadOrderDto>> GetAllByStatus(OrderStatus orderStatus)
        {
            return Ok(_orderManager.GetAllByStatus(orderStatus));
        }

        //Home Page
        [HttpGet]
        [Route("CountOrdersForEmployeeByStatus")]
        public ActionResult CountOrdersForEmployeeByStatus()
        {
            return Ok(_orderManager.CountOrdersForEmployeeByStatus());
        }

        [HttpGet]
        [Route("CountOrdersForMerchantByStatus")]
        public ActionResult CountOrdersForMerchantByStatus(string id)
        {
            return Ok(_orderManager.CountOrdersForMerchantByStatus(id));
        }

        
        //Paging
        [HttpGet]
        [Route("GetOrdersForEmployee")]
        public ActionResult<IEnumerable<ReadOrderDto>> GetOrdersForEmployee(int statusId, int pageNubmer, int pageSize, string searchText = "")
        {
            return Ok(_orderManager.GetOrdersForEmployee(searchText, statusId, pageNubmer, pageSize));
        }

        [HttpGet]
        [Route("GetOrdersForMerchant")]
        public ActionResult<IEnumerable<ReadOrderDto>> GetOrdersForMerchant(string merchantId, int statusId, int pageNubmer, int pageSize, string searchText = "")
        {
            return Ok(_orderManager.GetOrdersForMerchant(searchText, merchantId, statusId, pageNubmer, pageSize));
        }

        //Get Number of orders in every status 
        [HttpGet]
        [Route("GetCountOrdersForEmployee")]
        public ActionResult<int> GetCountOrdersForEmployee(int statusId, string searchText = "")
        {
            return Ok(_orderManager.GetCountOrdersForEmployee(statusId,searchText));
        }
        [HttpGet]
        [Route("GetCountOrdersForMerchant")]
        public ActionResult<int> GetCountOrdersForMerchant(string merchantId, int statusId, string searchText = "")
        {
            return Ok(_orderManager.GetCountOrdersForMerchant(merchantId, statusId, searchText));
        }


        [HttpPut]
        [Route("ChangeStatus")]
        public ActionResult ChangeStatus(int orderId, OrderStatus status)
        {
            bool result = _orderManager.ChangeStatus(orderId, status);
            if (result)
            {
                return Ok(new { message = "Changed Successfully" });

            }
            return BadRequest(new { message = "Item not found" });
        }

        [HttpPut]
        [Route("SelectRepresentative")]
        public ActionResult SelectRepresentative(int orderId, string representativeId)
        {
            bool result = _orderManager.SelectRepresentative(orderId, representativeId);
            if (result)
            {
                return Ok(new { message = "Selected Successfully" });

            }
            return BadRequest(new { message = "Item not found" });
        }

        //Orders For Representative
        [HttpGet]
        [Route("GetCountOrdersForRepresentative")]
        public ActionResult<int> GetCountOrdersForRepresentative(string representativeId, string searchText = "")
        {
            return Ok(_orderManager.GetCountOrdersForRepresentative(representativeId, searchText));
        }

        [HttpGet]
        [Route("GetOrdersForRepresentative")]
        public ActionResult<IEnumerable<ReadOrderDto>> GetOrdersForRepresentative(string representativeId, int pageNubmer, int pageSize, string searchText = "")
        {
            return Ok(_orderManager.GetOrdersForRepresentative(representativeId, pageNubmer, pageSize, searchText));
        }

        [HttpGet("DropdownListRepresentative")]
        public async Task<IActionResult> DropdownListRepresentative(int orderId)
        {
            try
            {
                return Ok(await _orderManager.DropdownListRepresentativeAsync(orderId));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetAllDataById")]
        public ActionResult<ReadAllOrderDataDto> GetAllDataById(int id)
        {
            var order = _orderManager.GetAllDataById(id);
            if (order != null)
            {
                return Ok(order);
            }
            return BadRequest(new { message = "Item not found" });
        }
    }
} 
