﻿using Key_Management_System.Enums;
using Key_Management_System.Services.RequestKeyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(Description ="Collector collects key")]
        public async Task<IActionResult> CollectKey(string key, Activity activity)
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
            //await _requestKeyService.CollectKey(key, activity, claimUser.Value);
            return Ok(await _requestKeyService.CollectKey(key, activity, claimUser.Value));
        }


        /// <summary>
        /// This controller help users to return key they have in their possesion
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("return-key")]
        [SwaggerOperation(Description ="Collector returns key")]
        public async Task<IActionResult> ReturnKey()
        {
            var claimUser = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication);
            //await _requestKeyService.ReturnKey(claimUser.Value);
            return Ok(await _requestKeyService.ReturnKey(claimUser.Value));
        }
    }
}
