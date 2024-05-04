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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> AssignCollectorKey(string key, [Required] General check)
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

            await _keyService.AssignCollectorKey(key, check, claimUser.Value);

            return Ok();
        }


        [HttpPut]
        [Route("accept-key")]
        [SwaggerOperation(Summary ="Worker accepts/declines key return")]
        public async Task<IActionResult> AcceptKeyReturn(string key, [Required] General check)
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

            await _keyService.AcceptKeyReturn(key, check, claimUser?.Value);

            return Ok();
        }
    }
}
