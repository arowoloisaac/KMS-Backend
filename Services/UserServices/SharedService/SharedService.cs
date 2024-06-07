using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.DTOs.UserDto.SharedDto;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Key_Management_System.Services.UserServices.SharedService
{
    public class SharedService : ISharedService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtBearerTokenSettings _bearerTokenSettings;
        private readonly IMapper _mapper;

        public SharedService(UserManager<User> userManager, IOptions<JwtBearerTokenSettings> jwtTokenOptions, IMapper mapper)
        {
            _userManager = userManager;
            _bearerTokenSettings = jwtTokenOptions.Value;
            _mapper = mapper;
        }

        public async Task<TokenResponse> Login(LoginDto request)
        {
            var user = await ValidateUser(request);

            if (user == null)
            {
                throw new InvalidOperationException("Login Failed");
            }

            var token = GenerateToken(user, await _userManager.GetRolesAsync(user));

            return new TokenResponse(token);
        }


        public async Task<ProfileDto> GetProfile(string Id)
        {
            var identifyUser = await _userManager.FindByIdAsync(Id);

            if (identifyUser == null)
            {
                return null;
            }

            var profile = new ProfileDto
            {
                FirstName = identifyUser.FirstName,
                LastName = identifyUser.LastName,
                Email = identifyUser.Email,
                PhoneNumber = identifyUser.PhoneNumber,
            };
            return profile;
        }


        public async Task UpdateProfile(UpdateProfileDto profile, string userId)
        {
            var identifyUser = await _userManager.FindByIdAsync(userId);

            if ( identifyUser == null)
            {
                throw new Exception("No user logged in");
            }

            else
            {
                identifyUser.LastName = profile.LastName;
                identifyUser.FirstName = profile.FirstName;
                identifyUser.PhoneNumber = profile.PhoneNumber;

                var updatedUser = await _userManager.UpdateAsync(identifyUser);

                if (!updatedUser.Succeeded)
                {
                    throw new Exception($"Failed to update profile: {userId}");
                }
            }
        }


        private async Task<User> ValidateUser(LoginDto request)
        {
            var identifyUser = await _userManager.FindByEmailAsync(request.Email);

            if (identifyUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identifyUser, identifyUser.PasswordHash, request.Password);

                return result == PasswordVerificationResult.Success ? identifyUser : null;
            }
            return null;
        }


        private string GenerateToken(User user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_bearerTokenSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Authentication, user.Id.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            /*new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Authentication, user.Id.ToString()),
                    
                    
                    //new Claim(ClaimTypes.Role, ApplicationRoleNames.Admin )
              }*/

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
