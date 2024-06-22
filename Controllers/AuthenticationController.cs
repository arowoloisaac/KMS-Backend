using Key_Management_System.Configuration;
using Key_Management_System.Enums;
using Key_Management_System.Services.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize(Roles =ApplicationRoleNames.Admin)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpGet]
        [Route("/users")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                return Ok(await _authenticationService.Users());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("/roles")]
        public async Task<IActionResult> AddRole([Required] Guid userId,[Required] Roles role)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                if (claimUser != null) {
                    return Ok(await _authenticationService.AddRole(userId, role, claimUser.Value));
                }

                else { return Unauthorized("user unauthorized"); }

            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("/roles")]
        public async Task<IActionResult> RemoveRole([Required] Guid userId, [Required] Roles role)
        {
            try
            {
                var user = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                if (user != null)
                {
                    return Ok(await _authenticationService.RemoveRole(userId, role, user.Value));
                }

                else { return Unauthorized("User unauthorized"); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
