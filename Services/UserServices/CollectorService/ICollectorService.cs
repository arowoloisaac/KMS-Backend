using Key_Management_System.DTOs.UserDto.KeyCollectorDto;
using Key_Management_System.Models;

namespace Key_Management_System.Services.UserServices.CollectorService
{
    public interface ICollectorService
    {
        Task<TokenResponse> RegisterCollector(RegisterCollectorDto registerCollectorDto);
    }
}
