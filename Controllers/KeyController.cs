using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Services.KeyService;
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
        [SwaggerOperation(Summary ="Add key to the list of keys")]
        public async Task<IActionResult> AddKey(AddKeyDto key)
        {
            var addKey = await _keyService.AddKey(key);  
            return Ok(addKey);
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
        public async Task<IActionResult> GetKey(Guid Id)
        {
            var getKey = await _keyService.GetKey(Id);
            return Ok(getKey);
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
        public async Task<IActionResult> GetKeys(KeyStatus? key)
        {
            var getKeys = await _keyService.GetKeys(key);
            return Ok(getKeys);
        }

        [HttpGet]
        [Route("key-with")]
        [AllowAnonymous]
        [SwaggerOperation(Summary ="Get the list to those with a key")]
        public async Task<IActionResult> KeyWith()
        {
            var getKeys = await _keyService.CheckKey();
            return Ok(getKeys);
        }
    }
}
