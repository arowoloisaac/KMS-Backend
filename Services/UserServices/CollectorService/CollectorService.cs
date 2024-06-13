using Key_Management_System.Configuration;
using Key_Management_System.DTOs.UserDto.KeyCollectorDto;
using Key_Management_System.Models;
using Key_Management_System.Services.UserServices.TokenService.TokenGenerator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Key_Management_System.Services.UserServices.CollectorService
{
    public class CollectorService : ICollectorService
    {
        private readonly UserManager<User> _collectorManager;
        private readonly JwtBearerTokenSettings _bearerTokenSettings;
        private readonly ITokenGenerator _tokenGenerator;

        public CollectorService(UserManager<User> collectorManager, IOptions<JwtBearerTokenSettings> tokenOption, ITokenGenerator tokenGenerator)
        {
            _collectorManager = collectorManager;
            _bearerTokenSettings = tokenOption.Value;
            _tokenGenerator = tokenGenerator;
        }


        public async Task<TokenResponse> RegisterCollector(RegisterCollectorDto collectorDto)
        {
            var existingUser = await _collectorManager.FindByEmailAsync(collectorDto.Email);



            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {existingUser.Email} already exist");
            }

            else
            {
                var createUser = await _collectorManager.CreateAsync(new KeyCollector
                {
                    LastName = collectorDto.LastName,
                    FirstName = collectorDto.FirstName,
                    Email = collectorDto.Email,
                    PhoneNumber = collectorDto.PhoneNumber,
                    UserName = collectorDto.Email,
                    Password = collectorDto.Password,
                }, collectorDto.Password);

                if (!createUser.Succeeded)
                {
                    throw new Exception("Unable to create account with those credentials");
                }

                else
                {
                    var createdUser = await _collectorManager.FindByEmailAsync(collectorDto.Email);

                    if (createdUser == null)
                    {
                        return null;
                    }
                    else
                    {
                        var token = _tokenGenerator.GenerateToken(createdUser);

                        return new TokenResponse(token);
                    }
                        
                }
            }
        }

        /*private string GenerateToken(User user)
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
        }*/
    }
}
