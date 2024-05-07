using Key_Management_System.Enums;
using Key_Management_System.Services.ThirdPartyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> SendRequest(Guid keyId, Activity activity)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                await _service.SendRequest(keyId, activity, claimUser.Value);

                return Ok("Key request sent");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("accept-request")]
        [SwaggerOperation(Summary ="Accept third party request")]
        public async Task<IActionResult> AcceptRequest(Guid keyId)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                await _service.AcceptRequest(keyId, claimUser.Value);

                return Ok("key accepted");
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
        public async Task<IActionResult> RejectRequest(Guid keyId) 
        {  
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                await _service.RejectRequest(keyId, claimUser.Value);

                return Ok("key rejected");
            }
            
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
