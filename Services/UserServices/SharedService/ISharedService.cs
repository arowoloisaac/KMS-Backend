using Key_Management_System.DTOs.UserDto.SharedDto;
using Key_Management_System.Models;

namespace Key_Management_System.Services.UserServices.SharedService
{
    public interface ISharedService
    {
        Task<TokenResponse> Login(LoginDto request);

        Task<ProfileDto> GetProfile(string Id);

        Task UpdateProfile(UpdateProfileDto profile, string userId);
    }
}
