using Key_Management_System.DTOs;
using Key_Management_System.Enums;

namespace Key_Management_System.Services.AssignKeyService
{
    public interface IAssignKeyService
    {
        Task AssignCollectorKey(string key, General check,string workerId);

        Task AcceptKeyReturn(string key, General check, string workerId);

        Task<List<KeyWith>> CheckRequest();
    }
}
