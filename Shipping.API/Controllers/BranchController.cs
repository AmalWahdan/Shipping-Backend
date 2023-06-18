﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL.Dtos;
using Shipping.BLL.Managers;
using Shipping.DAL.Params;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchManager _branchManager;

        public BranchController(IBranchManager branchManager)
        {
            _branchManager = branchManager;
        }


    
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowBranchDto>> GetById(int id)
        { 
            var branch = await _branchManager.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return Ok(branch);
        }

   
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowBranchDto>>> GetAll([FromQuery] GSpecParams branchSpecParams)
        {
            var branches = await _branchManager.GetAllBranchesAsync(branchSpecParams);
            return Ok(branches);
        }

      
        [HttpGet("all")]
        public async Task<ActionResult> GetAll()
        {
            var branches = await _branchManager.GetAllAsync();
            return Ok(branches);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(AddBranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _branchManager.CreateBranchAsync(branchDto);
            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);
        }

       
        [HttpPut]
        public async Task<IActionResult> Update(int id, UpdateBranchDto branchDto)
        {
            if (branchDto.Id != id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            var result = await _branchManager.UpdateBranchAsync(branchDto);
            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _branchManager.DeleteBranchAsync(id);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _branchManager.ChangeStatusBranchAsync(id);
            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);
        }

    }
}
