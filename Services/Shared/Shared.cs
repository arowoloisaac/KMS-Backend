using Key_Management_System.Configuration;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Key_Management_System.Services.Shared
{
    public class Shared: IShared
    {
        private readonly UserManager<User> _userManager;

        public Shared(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUser(string userId, string requiredRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not logged in or registered");
            }

            if (!await _userManager.IsInRoleAsync(user, requiredRole))
            {
                throw new UnauthorizedAccessException($"User is not in role {requiredRole}");
            }
            return user;
        }
    }
}
