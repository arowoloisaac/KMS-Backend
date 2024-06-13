using Key_Management_System.Models;

namespace Key_Management_System.Services.UserServices.TokenService.TokenGenerator
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user, IList<string> roles);

        //token for registration
        string GenerateToken(User user);
    }
}
