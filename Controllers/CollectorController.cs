using Key_Management_System.DTOs.UserDto.KeyCollectorDto;
using Key_Management_System.Services.UserServices.CollectorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors]
    public class CollectorController : ControllerBase
    {
        private readonly ICollectorService _collectorService;

        public CollectorController(ICollectorService collectorService)
        {
            _collectorService = collectorService;   
        }

        [HttpPost]
        [Route("register-collector")]
        [AllowAnonymous]
        [SwaggerOperation(Summary ="Key Collector registers an account")]
        public async Task<IActionResult> RegisterCollector(RegisterCollectorDto registerCollectorDto)
        {
            try
            {
                var getResponse = await _collectorService.RegisterCollector(registerCollectorDto);
                return Ok(getResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
