﻿using Microsoft.AspNetCore.Mvc;
using Shipping.BLL.Dtos;
using Shipping.BLL.Managers;
using Shipping.DAL.Params;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupManager _groupManager;
        public GroupController(IGroupManager groupManager)
        {
            _groupManager = groupManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto groupDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _groupManager.AddGroupAsync(groupDto);
            if (result > 0)
                return Ok(new { message = "Done" });
            return StatusCode(500);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetGroups([FromQuery] GSpecParams groupSpecParams)
        {
            var groups = await _groupManager.GetAllAsync(groupSpecParams);
            return Ok(groups);

        }

        [HttpGet("all")]
       
        public async Task<IActionResult> GetGroupsForSelect()
        {
            var groups = await _groupManager.GetAllAsync();
            return Ok(groups);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupWithPermissions(int id)
        {
            var group = await _groupManager.GetGroupByIdWithPermissionsAsync(id);
            if (group == null)
                return NotFound();
            return Ok(group);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDto groupDto)
        {
            if (id != groupDto.Id)
                return BadRequest();
            var result = await _groupManager.UpdateGroupAsync(id, groupDto);
            if (result == 0)
                return NotFound();
            else if (result == -1)
                return BadRequest(new { message = "Invalid GovernorateId" });
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var result = await _groupManager.DeleteGroupAsync(id);
            if (result == 0)
            {
                return NotFound();
            }
            if (result == -1)
            {
                return Ok(new { Message = "Delete Cities First" });

            }

            return Ok();
        }
    }
}
