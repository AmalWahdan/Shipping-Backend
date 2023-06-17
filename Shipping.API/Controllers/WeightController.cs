﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL;
using Shipping.BLL.Dtos;
using Shipping.DAL.Data.Models;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private readonly IWeightsManager _weightsManager;

        public WeightController(IWeightsManager weightsManager)
        {
            this._weightsManager = weightsManager;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Weight>> GetShippingTypeById(int id)
        {
            var weight = await _weightsManager.GetWeightByIdAsync(id);
            if (weight== null)
            {
                return NotFound();
            }

            return Ok(weight);
        }
        [HttpPost]
        public async Task<ActionResult> Add(AddWeightDto weight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =await _weightsManager.Add(weight);
            if (result >0)
            {
                return Ok("Weight was added successfully.");
            }
            ModelState.AddModelError("save", "Can't save Weight may be something wrong!");
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateWeightDtos weight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =await _weightsManager.Update(weight);
            if (result >0)
            {
                return Ok(new { message = "Weight was updated successfully." });
            }
            ModelState.AddModelError("save", "Can't save Weight may be something wrong!");
            return BadRequest(ModelState);
        }
    }
}
