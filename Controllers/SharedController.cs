using Key_Management_System.DTOs.UserDto.SharedDto;
using Key_Management_System.Services.UserServices.SharedService;
using Key_Management_System.Services.UserServices.TokenService;
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
    [EnableCors]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _userService;
        private ITokenStorageService _tokenStorageService;

        public SharedController(ISharedService sharedService, ITokenStorageService tokenStorageService)
        {
            _userService = sharedService;
            _tokenStorageService = tokenStorageService;
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        [SwaggerOperation(Summary ="User view their profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("User is not authenticated.");
                }

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
        [SwaggerOperation(Summary ="User update their profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto profileDto)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(user => user.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("User is not authenticated.");
                }

                await _userService.UpdateProfile(profileDto, claimUser.Value);
                return StatusCode(200);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var id = Guid.Parse(User.FindFirst(ClaimTypes.Authentication).Value);

            _tokenStorageService.LogoutToken(id);
            return Ok("logout successful");
        }
    }
}
