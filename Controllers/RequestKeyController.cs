﻿using Key_Management_System.Configuration;
using Key_Management_System.Enums;
using Key_Management_System.Services.RequestKeyService;
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
    [Authorize(Roles= ApplicationRoleNames.Collector)]
    [EnableCors]
    public class RequestKeyController : ControllerBase
    {
        private readonly IRequestKeyService _requestKeyService;

        public RequestKeyController(IRequestKeyService requestKeyService)
        {
            _requestKeyService = requestKeyService;
        }

        [HttpPost]
        [Route("collect-key")]
        [SwaggerOperation(Summary ="Collector collects key")]
        public async Task<IActionResult> CollectKey([Required]Guid keyId, [Required]Activity activity)
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);

                if(claimUser == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                return Ok(await _requestKeyService.CollectKey(keyId, activity, claimUser.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This controller help users to return key they have in their possesion
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("return-key")]
        [SwaggerOperation(Summary ="Collector returns key")]
        public async Task<IActionResult> ReturnKey()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                return Ok(await _requestKeyService.ReturnKey(claimUser.Value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("with-you")]
        [SwaggerOperation(Summary ="Get key with you")]
        public async Task<IActionResult> YourKey()
        {
            try
            {
                var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
                if (claimUser == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                return Ok(await _requestKeyService.GetView(claimUser.Value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
