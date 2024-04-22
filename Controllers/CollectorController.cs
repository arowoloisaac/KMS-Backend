﻿using Key_Management_System.DTOs.UserDto.KeyCollectorDto;
using Key_Management_System.Services.UserServices.CollectorService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Key_Management_System.Controllers
{
    [Route("api/")]
    [ApiController]
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
        public async Task<IActionResult> RegisterCollector(RegisterCollectorDto registerCollectorDto)
        {
            await _collectorService.RegisterCollector(registerCollectorDto);
            return Ok(registerCollectorDto);
        }
    }
}