using Key_Management_System.Configuration;
using Key_Management_System.Enums;
using Key_Management_System.Services.AssignKeyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles =ApplicationRoleNames.Manager)]
    [EnableCors]
    public class AssignKeyController : ControllerBase
    {
        private readonly IAssignKeyService _keyService;

        public AssignKeyController(IAssignKeyService assignKeyService)
        {
            _keyService = assignKeyService;
        }


        [HttpPut]
        [Route("assign-key")]
        [SwaggerOperation(Summary ="Worker assign key to collector")]
        public async Task<IActionResult> AssignCollectorKey([Required]Guid keyId, [Required] General check)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                if (claimUser == null) { 
                    return Unauthorized("user not authorized");
                }

                return Ok(await _keyService.AssignCollectorKey(keyId, check, claimUser.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        [HttpPut]
        [Route("accept-key")]
        [SwaggerOperation(Summary ="Worker accepts/declines key return")]
        public async Task<IActionResult> AcceptKeyReturn([Required]Guid keyId, [Required] General check)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("user not authorized");
                }
                return Ok(await _keyService.AcceptKeyReturn(keyId, check, claimUser?.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
            
        }

        [HttpGet]
        [Route("requests")]
        [SwaggerOperation(Summary ="Check for key requests")]
        public async Task<IActionResult> KeyWith()
        {
            try
            {
                var getKeys = await _keyService.CheckRequest();
                return Ok(getKeys);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("returns")]
        [SwaggerOperation(Summary = "Check for key return requests")]
        public async Task<IActionResult> KeyReturn()
        {
            try
            {
                var getKeys = await _keyService.CheckReturns();
                return Ok(getKeys);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
