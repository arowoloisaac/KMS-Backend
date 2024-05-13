using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.DTOs.UserDto.WorkerDto;
using Key_Management_System.Models;
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

        public WorkerService(UserManager<User> workerManager, IOptions<JwtBearerTokenSettings> jwtTokenOptions)
        {
            _workerManager = workerManager;
            _bearerTokenSettings = jwtTokenOptions.Value;
        }


        public async Task<TokenResponse> RegisterWorker(RegisterWorkerDto workerDto)
        {
            var existingUser = await _workerManager.FindByEmailAsync(workerDto.Email);

            var checkAdminExistence = await _workerManager.FindByNameAsync("Administrator");

            if (checkAdminExistence == null)
            {
                var AdminUser = new Worker
                {
                    FirstName = "Administrator",
                    Email = "admin@gmail.com",
                    UserName = "Administrator",
                    Password = "example123"
                };

                var result = await _workerManager.CreateAsync(AdminUser, "example123");

                if (!result.Succeeded)
                {
                    throw new Exception("Unable to create user admin");
                }
                else
                {
                    var token = GenerateToken(AdminUser);
                    return new TokenResponse(token);
                }
            }


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
                        return null;
                    }

                    else
                    {
                        var token = GenerateToken(getCreatedUser);
                        return new TokenResponse(token);
                    }
                }
            }

        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_bearerTokenSettings.SecretKey);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Authentication, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddSeconds(_bearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _bearerTokenSettings.Audience,
                Issuer = _bearerTokenSettings.Issuer,
            };

            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
