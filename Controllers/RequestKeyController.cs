using Key_Management_System.Enums;
using Key_Management_System.Services.RequestKeyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class RequestKeyController : ControllerBase
    {
        private readonly IRequestKeyService _requestKeyService;

        public RequestKeyController(IRequestKeyService requestKeyService)
        {
            _requestKeyService = requestKeyService;
        }

        [HttpPost]
        [Route("collect-key")]
        public async Task<IActionResult> CollectKey(string key, Activity activity)
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
            await _requestKeyService.CollectKey(key, activity, claimUser.Value);
            return Ok();
        }


        [HttpPut]
        [Route("return-key")]
        public async Task<IActionResult> ReturnKey()
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
            await _requestKeyService.ReturnKey(claimUser.Value);
            return Ok();
        }
    }
}
