using Key_Management_System.DTOs;
using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.AssignKeyService
{
    public interface IAssignKeyService
    {
        Task<Message> AssignCollectorKey(Guid keyId, General check,string workerId);

        Task<Message> AcceptKeyReturn(Guid keyId, General check, string workerId);

        Task<List<KeyCollectorRequest>> CheckRequest();

        Task<List<KeyCollectorRequest>> CheckReturns();
    }
}
