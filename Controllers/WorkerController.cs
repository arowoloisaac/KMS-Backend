﻿using Key_Management_System.DTOs.UserDto.WorkerDto;
using Key_Management_System.Services.UserServices.WorkerService;
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
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkerController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [HttpPost]
        [Route("register-worker")]
        [AllowAnonymous]
        [SwaggerOperation(Summary ="Worker registers account")]
        public async Task<IActionResult> Register(RegisterWorkerDto workerDto)
        {
            var getResponse = await _workerService.RegisterWorker(workerDto);
            return Ok(getResponse);
        }
    }
}
