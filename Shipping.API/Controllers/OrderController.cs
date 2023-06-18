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
        [Route("CountAll")]
        public ActionResult<int> CountAll()
        {
            return Ok(_orderManager.CountAll());
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
    }
} 
