using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.RequestKeyService
{
    public interface IRequestKeyService
    {
        Task<Message> CollectKey( string key, Activity activity ,string userId );  

        Task<Message> ReturnKey(string userId);
    }
}
