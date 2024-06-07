using Key_Management_System.DTOs.AuthenticationDto;
using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<List<UsersDto>> Users();

        Task<Message> AddRole(Guid userId, Roles role, string adminId);

        Task<Message> RemoveRole(Guid userId, Roles role, string adminId);
    }
}
