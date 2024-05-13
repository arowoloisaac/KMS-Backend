using Key_Management_System.DTOs.UserDto.WorkerDto;
using Key_Management_System.Models;

namespace Key_Management_System.Services.UserServices.WorkerService
{
    public interface IWorkerService
    {
        Task<TokenResponse> RegisterWorker(RegisterWorkerDto workerDto);
    }
}
