﻿using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.DTOs;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.DTOs.RequestKey;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Key_Management_System.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Key_Management_System.Services.KeyService
{
    public class KeyService : IkeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IShared _shared;

        private string requiredRole = ApplicationRoleNames.Admin;

        public KeyService(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, IShared shared)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _shared = shared;
        }


        public async Task<Key> AddKey(AddKeyDto key, string adminId)
        {
            var checkAdmin = await _shared.GetUser(adminId, requiredRole);

            var checkKey = await GetRoom(key.Room);

            if (checkKey == null)
            {
                var dto = _mapper.Map<Key>(key);
                dto.Id = Guid.NewGuid();
                var addToDb = await _context.Key.AddAsync(dto);
                await _context.SaveChangesAsync();

                return addToDb.Entity;
            }
            else
            {
                throw new Exception("Room Number should have a value");
            }
        }


        public async Task UpdateKey(string oldName, string newName, string adminId)
        {
            var checkAdmin = await _shared.GetUser(adminId, requiredRole);

            var findKey = await GetRoom(oldName);

            if (findKey != null)
            {
                findKey.Room = newName;
                _context.Key.Update(findKey);

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Key with name: {oldName} not found in the database");
            }
        }

        public async Task DeleteKey(Guid keyId, string adminId)
        {
            var checkAdmin = await _shared.GetUser(adminId, requiredRole);

            var checkKey = await _context.Key.FindAsync(keyId);

            if (checkKey is not null)
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
                throw new Exception($"unable to remove key: {checkKey.Room} from database");
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
                AssignedTime = classRoom.AssignedTime,
            };
            return new List<KeyWith> { getReponse };
        }

        private async Task<Key> GetRoom(string RoomNumber)
        {
            var getRoom = await _context.Key.FirstOrDefaultAsync(filter => filter.Room == RoomNumber);
            return getRoom;
        }
    }
}
