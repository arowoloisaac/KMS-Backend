using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.RequestKeyService
{
    public interface IRequestKeyService
    {
        Task<Message> CollectKey(Guid keyId, Activity activity ,string userId );  

        Task<Message> ReturnKey(string userId);

        Task<ViewUsage> GetView(string userId);

        //Task GetRequests(); // this will aim just to get usser with key inother to aim the frontend.
    }
}
