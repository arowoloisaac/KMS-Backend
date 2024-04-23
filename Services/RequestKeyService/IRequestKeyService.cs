using Key_Management_System.Enums;

namespace Key_Management_System.Services.RequestKeyService
{
    public interface IRequestKeyService
    {
        Task CollectKey( string key, Activity activity ,string userId );  

        Task ReturnKey(string userId);
    }
}
