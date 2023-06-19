﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL.Dtos;
using Shipping.BLL.Managers;
using Shipping.DAL.Params;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernorateController : ControllerBase
    {
        private readonly IGovernorateManager _governorateManager;

        public GovernorateController(IGovernorateManager governorateManager)
        {
            _governorateManager = governorateManager;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowGovernorateDto>>> GetAll()
        {
            var governorates = await _governorateManager.GetAllGovernorateAsync();
            return Ok(governorates);
        }


        [HttpGet("allWithCity")]
        public async Task<ActionResult<IEnumerable<UpdateGovernorateDto>>> GetAllWithCity()
        {
            var governorates = await _governorateManager.GetAllGovernorateWithCityAsync();
            return Ok(governorates);
        }
        [HttpGet("allCitiesWithGovernorate")]
        public async Task<ActionResult<IEnumerable<UpdateGovernorateDto>>> GetAllWithCities()
        {
            var governorates = await _governorateManager.GetAllGovernorateWithCitiesAsync();
            return Ok(governorates);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ShowGovernorateDto>>> GetAllWithDeleted([FromQuery] GSpecParams govSpecParams)
        {
            var governorates = await _governorateManager.GetAllGovernorateWithDeletedAsync(govSpecParams);
            return Ok(governorates);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(AddGovernorateDto governorateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _governorateManager.CreateGovernorateAsync(governorateDto);
            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateGovernorateDto governorateDto)
        {
            if (id != governorateDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _governorateManager.UpdateGovernorateAsync(governorateDto);
            if (result > 0)
            {
                return Ok();
            }


            return StatusCode(500);

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _governorateManager.DeleteGovernorateAsync(id);

            if (result == 0)
            {
                return NotFound();
            }
            if (result == -1)
            {
                return Ok(new { Message = "Delete Employee First" });

            }

            return Ok();

        }

       



    }
}
