using Key_Management_System.DTOs;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.DTOs.RequestKey;
using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.KeyService
{
    public interface IkeyService
    {
        Task<Key> AddKey(AddKeyDto key);

        Task UpdateKey(string oldName, string newName);

        Task DeleteKey(Guid keyId);

        Task<IEnumerable<GetKeyDto>> GetKeys(KeyStatus? status);

        Task<GetKeyDto> GetKey(Guid Id);

        Task<List<KeyWith>> CheckKey();
    }
}
