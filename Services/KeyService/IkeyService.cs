using Key_Management_System.DTOs;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.DTOs.RequestKey;
using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.KeyService
{
    public interface IkeyService
    {
        Task<Key> AddKey(AddKeyDto key, string adminId);

        Task UpdateKey(string oldName, string newName, string adminId);

        Task DeleteKey(Guid keyId, string adminId);

        Task<IEnumerable<GetKeyDto>> GetKeys(KeyStatus? status);

        Task<GetKeyDto> GetKey(Guid Id);

        Task<List<KeyWith>> CheckKey();
    }
}
