using Key_Management_System.Configuration;
using Key_Management_System.Enums;
using Key_Management_System.Services.ThirdPartyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles =ApplicationRoleNames.Collector)]
    [EnableCors]
    public class ThirdPartyController : ControllerBase
    {
        private readonly IThirdPartyService _service;

        public ThirdPartyController(IThirdPartyService thirdPartyService)
        {
            _service = thirdPartyService;
        }


        [HttpPost]
        [Route("Send-request")]
        [SwaggerOperation(Summary ="Send requst to access key via third party means")]
        public async Task<IActionResult> SendRequest([Required] Guid keyId, [Required]Activity activity)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                var response = await _service.SendRequest(keyId, activity, claimUser.Value);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("accept-request")]
        [SwaggerOperation(Summary ="Accept third party request")]
        public async Task<IActionResult> AcceptRequest([Required] Guid keyId)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                var response = await _service.AcceptRequest(keyId, claimUser.Value);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-request")]
        [SwaggerOperation(Summary ="Get notification about request")]
        public async Task<IActionResult> GetRequest()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                var response = await _service.GetRequest(claimUser.Value);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-notifier")]
        [SwaggerOperation(Summary ="Returns boolean value to show notification")]
        public async Task<IActionResult> ReturnNotification()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                var response = await _service.Notifier(claimUser.Value);

                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("reject-request")]
        [SwaggerOperation(Summary ="Reject third party request")]
        public async Task<IActionResult> RejectRequest([Required] Guid keyId) 
        {  
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                var response = await _service.RejectRequest(keyId, claimUser.Value);

                return Ok(response);
            }
            
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
