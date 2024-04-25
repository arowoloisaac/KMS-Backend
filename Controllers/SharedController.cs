using Key_Management_System.DTOs.UserDto.SharedDto;
using Key_Management_System.Services.UserServices.SharedService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _userService;

        public SharedController(ISharedService sharedService)
        {
            _userService = sharedService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Log in to the system")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                return Ok(await _userService.Login(model));
            }
            catch (InvalidOperationException ex)
            {
                // Write logs
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        [SwaggerOperation(Description ="User view their profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                var getUser = await _userService.GetProfile(claimUser.Value);

                return Ok(getUser);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Route("profile")]
        [Authorize]
        [SwaggerOperation(Description ="User update their profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto profileDto)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(user => user.Type == ClaimTypes.Authentication);

                await _userService.UpdateProfile(profileDto, claimUser.Value);

               return StatusCode(200);
            }

            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
