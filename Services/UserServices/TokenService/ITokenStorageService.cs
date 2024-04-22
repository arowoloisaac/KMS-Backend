namespace Key_Management_System.Services.UserServices.TokenService
{
    public interface ITokenStorageService
    {
        void LogoutToken(Guid identifier);
        bool CheckIfTokenIsLoggedOut(Guid identifier);
    }
}
