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
        public async Task<ActionResult<AddOrderResultDto>> Add(AddOrderDto order)
        {
            var result = await _orderManager.Add(order);
            if (result.IsSuccesfull && ModelState.IsValid)
            {
                return Ok(new { message = "Order was added successfully.",result });
            }
            ModelState.AddModelError("save", "Can't save Order may be some ID'S wrong!");
            return BadRequest(ModelState);
        }
                
        [HttpPut]
        public async Task<ActionResult<UpdateOrderResultDto>> Update(UpdateOrderDto order)
        {
            var result =await _orderManager.Update(order);
            if (result.IsSuccesfull && ModelState.IsValid)
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

        [HttpGet]
        [Route("CountOrdersForRepresentativeByStatus")]
        public ActionResult CountOrdersForRepresentativeByStatus(string representativeId)
        {
            return Ok(_orderManager.CountOrdersForRepresentativeByStatus(representativeId));
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
        public ActionResult<int> GetCountOrdersForRepresentative(string representativeId,int statusId, string searchText = "")
        {
            return Ok(_orderManager.GetCountOrdersForRepresentative(representativeId,statusId, searchText));
        }

        [HttpGet]
        [Route("GetOrdersForRepresentative")]
        public ActionResult<IEnumerable<ReadOrderDto>> GetOrdersForRepresentative(string representativeId, int statusId, int pageNubmer, int pageSize, string searchText = "")
        {
            return Ok(_orderManager.GetOrdersForRepresentative(representativeId,statusId, pageNubmer, pageSize, searchText));
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

        [HttpPut]
        [Route("ChangeStatusAndReasonRefusal")]
        public ActionResult ChangeStatusAndReasonRefusal(int orderId, OrderStatus status,int? reasonRefusal)
        {
            bool result = _orderManager.ChangeStatusAndReasonRefusal(orderId, status,reasonRefusal);
            if (result)
            {
                return Ok(new { message = "Changed Successfully" });

            }
            return BadRequest(new { message = "Item not found" });
        }
    }
} 
