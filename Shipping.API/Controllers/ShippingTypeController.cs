using Microsoft.AspNetCore.Mvc;
using Shipping.BLL;
using Shipping.BLL.Dtos;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingTypeController : ControllerBase
    {
        private readonly IShippingTypeManager _shippingTypeManager;

        public ShippingTypeController(IShippingTypeManager shippingTypeManager)
        {
            _shippingTypeManager = shippingTypeManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShowShippingTypeDto>> GetShippingTypeById(int id)
        {
            var shippingType = await _shippingTypeManager.GetShippingTypeByIdAsync(id);
            if (shippingType == null)
            {
                return NotFound();
            }

            return Ok(shippingType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllTypesDto>>> GetAllShippingTypes()
        {
            var shippingTypes = await _shippingTypeManager.GetAllShippingTypeAsync();

            return Ok(shippingTypes);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ShowShippingTypeDto>>> GetAllShippingTypesWithDeleted()
        {
            var shippingTypes = await _shippingTypeManager.GetAllShippingTypeWithDeletedAsync();

            return Ok(shippingTypes);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateShippingType(AddShippingTypeDto shippingTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _shippingTypeManager.CreateShippingTypeAsync(shippingTypeDto);

            if (result > 0)
            {
                return Ok();
            }


            return StatusCode(500);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateShippingType(int id, UpdatShippingTypeDto shippingTypeDto)
        {
            if (id != shippingTypeDto.Id)
            {
                return BadRequest();
            }

            var result = await _shippingTypeManager.UpdateShippingTypeAsync(shippingTypeDto);

            if (result > 0)
            {
                return Ok();
            }


            return StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteShippingType(int id)
        {
            var result = await _shippingTypeManager.DeleteShippingTypeAsync(id);

            if (result > 0)
            {
                return Ok();
            }


            return StatusCode(500);
        }

        [HttpDelete("changeState")]
        public async Task<ActionResult<int>> ToggleShippingType(int id)
        {
            var result = await _shippingTypeManager.ToggleShippingTypeStatusAsync(id);

            if (result > 0)
            {
                return Ok();
            }


            return StatusCode(500);
        }
    }
}
