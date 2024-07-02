using Key_Management_System.Configuration;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Services.KeyService;
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
    [EnableCors]
    public class KeyController : ControllerBase
    {
        private readonly IkeyService _keyService;

        public KeyController(IkeyService keyService)
        {
            _keyService = keyService;
        }

        /// <summary>
        /// This function aims to add key to the list of keys in the database
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// {
        ///     id: Guid,
        ///     Room: Room number
        ///     Status: availability
        /// }
        /// </returns>
        [HttpPost]
        [Route("add-key")]
        [Authorize(Roles = ApplicationRoleNames.Admin)]
        [SwaggerOperation(Summary ="Add key to the list of keys")]
        public async Task<IActionResult> AddKey(AddKeyDto key)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("user not authorized");
                }
                var addKey = await _keyService.AddKey(key, claimUser.Value);
                return Ok(addKey);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update-key")]
        [SwaggerOperation(Summary = "update key to the list of keys")]
        [Authorize(Roles = ApplicationRoleNames.Admin)]
        public async Task<IActionResult> UpdateKey([Required] string oldName,[Required] string newName)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("user not authorized");
                }
                await _keyService.UpdateKey(oldName, newName, claimUser.Value);
                return Ok("Key updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("remove-key")]
        [SwaggerOperation(Summary = "remove key to the list of keys")]
        [Authorize(Roles = ApplicationRoleNames.Admin)]
        public async Task<IActionResult> DeleteKey([Required] Guid keyId)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("user not authorized");
                }
                await _keyService.DeleteKey(keyId, claimUser.Value);
                return Ok("key removed from database");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }


        /// <summary>
        /// This function returns a key item from the database, this function aids user to search for a particular key
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>
        /// / Get 
        /// {
        ///     Id: key id,
        ///     Room: RoomNumber string,
        ///     Status: Availability Enum
        /// }
        /// </returns>
        [HttpGet]
        [Route("get-key-id")]
        [SwaggerOperation(Summary ="Get Key via its respective Id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKey(Guid Id)
        {
            try
            {
                var getKey = await _keyService.GetKey(Id);
                return Ok(getKey);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// This function returns the filtered/nor-filtered(depending on user selection) list of keys
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// / Get 
        /// {
        ///     Id: key id,
        ///     Room: RoomNumber string,
        ///     Status: Availability Enum
        /// }
        /// </returns>
        [HttpGet]
        [Route("get-keys")]
        [SwaggerOperation(Summary ="Get list of keys either by filtering the availabity or no filtering")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKeys(KeyStatus? key)
        {
            try
            {
                var getKeys = await _keyService.GetKeys(key);
                return Ok(getKeys);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("key-with")]
        [AllowAnonymous]
        [SwaggerOperation(Summary ="Get the list to those with a key")]
        public async Task<IActionResult> KeyWith()
        {
            try
            {
                var getKeys = await _keyService.CheckKey();
                return Ok(getKeys);
            }
            catch (Exception e1)
            {
                return BadRequest(e1.Message);
            }
        }
    }
}
