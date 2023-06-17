using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.BLL.Dtos;
using Shipping.BLL.Managers;
using Shipping.DAL.Params;

namespace Shipping.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {

            private readonly IMerchantManager _merchantManager;

            public MerchantController(IMerchantManager merchantManager)
            {
                _merchantManager = merchantManager;
            }

            [HttpPost]
            public async Task<IActionResult> RegisterMerchant(MerchantRegisterDto registrationDto)
            {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _merchantManager.RegisterMerchant(registrationDto);

            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);

        }


           [HttpPut]
          public async Task<IActionResult> UpdateMerchant(string id, MerchantUpdateDto updateDto)
            {

            if (id != updateDto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _merchantManager.UpdateMerchant(updateDto);

            if (result > 0)
            {
                return Ok();
            }

            return StatusCode(500);
        }

           [HttpDelete]
            public async Task<IActionResult> DeleteMerchant(string id)
            {
                var result = await _merchantManager.DeleteMerchant(id);

            if (result > 0)
            {
                return Ok();
            }
            return StatusCode(500);
        }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetMerchantById(string id)
            {
                var merchant = await _merchantManager.GetMerchantByIdWithSpecialPrices(id);

                if (merchant == null)
                {
                    return NotFound();
                }

                return Ok(merchant);
            }

            [HttpGet]
            public async Task<IActionResult> GetAllMerchants([FromQuery] GSpecParams merchantSpecParams)
            {
                var merchants = await _merchantManager.GetAllMarchentsAsync(merchantSpecParams);
                return Ok(merchants);
            }
        }
    }

