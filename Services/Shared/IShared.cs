using Key_Management_System.Models;

namespace Key_Management_System.Services.Shared
{
    public interface IShared
    {
        Task<User> GetUser(string userId, string requiredRole);
    }
}
