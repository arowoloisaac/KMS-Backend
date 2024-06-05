using AutoMapper;
using Key_Management_System.Data;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.DTOs.RequestKey;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Key_Management_System.Services.KeyService
{
    public class KeyService : IkeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public KeyService(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }


        public async Task<Key> AddKey(AddKeyDto key)
        {
            var checkAdmin = await _userManager.FindByEmailAsync("admin@gmail.com");
            var checkKey = await _context.Key.FirstOrDefaultAsync(check => check.Room == key.Room);

            if (checkKey == null && checkAdmin is Worker)
            {
                var dto = _mapper.Map<Key>(key);
                dto.Id = Guid.NewGuid();
                var addToDb = await _context.Key.AddAsync(dto);
                await _context.SaveChangesAsync();

                return addToDb.Entity;
            }
            
            else
            {
                throw new InvalidOperationException("Key already exist in database or invalid inputs");
            }
        }


        public async Task UpdateKey(string oldName, string newName)
        {
            var checkAdmin = await _userManager.FindByEmailAsync("admin@gmail.com");

            if (checkAdmin != null && checkAdmin is Worker)
            {
                var findKey = await _context.Key.FirstOrDefaultAsync(find => find.Room == oldName);

                if (findKey != null)
                {
                    findKey.Room = newName;
                    _context.Key.Update(findKey);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new KeyNotFoundException("Key not found.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Only workers are allowed to update keys.");
            }
        }

        public async Task DeleteKey(Guid keyId)
        {
            var checkAdmin = await _userManager.FindByEmailAsync("admin@gmail.com");
            var checkKey = await _context.Key.FindAsync(keyId);

            if (checkKey is not null && checkAdmin is Worker)
            {
                var associatedRequest = _context.RequestKey.Where(x => x.Key.Id == keyId);

                foreach (var request in associatedRequest)
                {
                    _context.RequestKey.Remove(request);
                }
                _context.Key.Remove(checkKey);

                await _context.SaveChangesAsync();
            }
            
            else
            {
                throw new Exception("unable to remove");
            }
        }

        public async Task<GetKeyDto> GetKey(Guid Id)
        {
            var findKey = await _context.Key.FindAsync(Id);

            if (findKey != null)
            {
                var dto = _mapper.Map<GetKeyDto>(findKey);

                return dto;
            }

            else
            {
                throw new Exception($"Key with id - {Id} doesn't exist");
            }
        }


        public async Task<IEnumerable<GetKeyDto>> GetKeys(KeyStatus? status)
        {
            IQueryable<Key> query = _context.Key;

            if (status.HasValue)
            {
                query = query.Where(key => key.Status == status);
            }

            var keys = await query.ToListAsync();   

            var mappedKeys = _mapper.Map<IEnumerable<GetKeyDto>>(keys);

            return mappedKeys;
        }


        public async Task<List<KeyWith>> CheckKey()
        {
            var classRoom = await _context.RequestKey.Where(filter => filter.Availability == CheckWith.InHand).FirstOrDefaultAsync();

            if (classRoom == null)
            {
                return new List<KeyWith>();
            }


            var getReponse = new KeyWith
            {
                CollectorId = classRoom.KeyCollectorId,
                Room = classRoom._Key,
                Activity = classRoom.Activity,
                CollectionTime = classRoom.CollectionTime,
            };

            return new List<KeyWith> { getReponse };
        }

    }
}
