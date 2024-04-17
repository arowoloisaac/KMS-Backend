using AutoMapper;
using Key_Management_System.Data;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Key_Management_System.Services.KeyService
{
    public class KeyService : IkeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public KeyService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<Key> AddKey(AddKeyDto key)
        {
            var checkKey = await _context.Key.FirstOrDefaultAsync(check => check.Id == key.Id || check.Room == key.Room);

            if (checkKey == null)
            {
                var dto = _mapper.Map<Key>(key);
                var addToDb = await _context.Key.AddAsync(dto);
                await _context.SaveChangesAsync();

                return addToDb.Entity;
            }
            
            else
            {
                throw new InvalidOperationException("Key already exist in database or invalid inputs");
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
    }
}
