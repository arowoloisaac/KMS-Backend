using Key_Management_System.DTOs.UserDto.KeyCollectorDto;

namespace Key_Management_System.Services.UserServices.CollectorService
{
    public interface ICollectorService
    {
        Task RegisterCollector(RegisterCollectorDto registerCollectorDto);
    }
}
