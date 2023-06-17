
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL.Dtos.CityDtos;
using Shipping.BLL.Dtos;
using Shipping.BLL.Managers;
using Shipping.API.Filters;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityManager _cityManager;

        public CityController(ICityManager cityManager)
        {
            _cityManager = cityManager;
        }


        [HttpGet]
        [TypeFilter(typeof(GpAttribute))]
        public async Task<ActionResult<IEnumerable<ShowCityDto>>> GetAllCities()
        {
            var cities = await _cityManager.GetAllCityWithDeletedAsync();
            return Ok(cities);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateCity(AddCityDto cityDto)
        {
            var result = await _cityManager.CreateCityAsync(cityDto);
            if (result == -1)
            {
                return BadRequest("Invalid GovernorateId");
            }

            return Ok();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateCity(int id, UpdateCityDto cityDto)
        {
            if (id != cityDto.Id)
            {
                return BadRequest();
            }

            var result = await _cityManager.UpdateCityAsync(cityDto);
            if (result == 0)
            {
                return NotFound();
            }
            else if (result == -1)
            {
                return BadRequest("Invalid GovernorateId");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteCity(int id)
        {
            var result = await _cityManager.DeleteCityAsync(id);
            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);
        }
    }
}
