using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.DTOs.UserDto.WorkerDto;
using Key_Management_System.Models;
using Key_Management_System.Services.UserServices.TokenService.TokenGenerator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Key_Management_System.Services.UserServices.WorkerService
{
    public class WorkerService : IWorkerService
    {
        private UserManager<User> _workerManager;
        private readonly JwtBearerTokenSettings _bearerTokenSettings;
        private readonly ITokenGenerator _tokenGenerator;

        public WorkerService(UserManager<User> workerManager, IOptions<JwtBearerTokenSettings> jwtTokenOptions, ITokenGenerator tokenGenerator)
        {
            _workerManager = workerManager;
            _bearerTokenSettings = jwtTokenOptions.Value;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<TokenResponse> RegisterWorker(RegisterWorkerDto workerDto)
        {
            var existingUser = await _workerManager.FindByEmailAsync(workerDto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {existingUser.Email} already exist");   
            }

            else
            {
                var createUser = await _workerManager.CreateAsync(new Worker
                {
                    FirstName = workerDto.FirstName,
                    LastName = workerDto.LastName,
                    Email = workerDto.Email,
                    PhoneNumber = workerDto.PhoneNumber,
                    Faculty = workerDto.Faculty,
                    UserName = workerDto.Email,
                    Password = workerDto.Password,
                }, workerDto.Password);


                if (!createUser.Succeeded)
                {
                    throw new Exception("Unable to create account with those credentials");
                }

                else
                {
                    var getCreatedUser = await _workerManager.FindByEmailAsync(workerDto.Email);
                    if (getCreatedUser == null)
                    {
                        throw new Exception("Can't find the user with email");
                    }

                    else
                    {
                        var token = _tokenGenerator.GenerateToken(getCreatedUser);
                        return new TokenResponse(token);
                    }
                }
            }

        }
    }
}
