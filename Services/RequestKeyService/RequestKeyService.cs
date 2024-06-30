using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.DTOs;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Key_Management_System.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Key_Management_System.Services.RequestKeyService
{
    public class RequestKeyService : IRequestKeyService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IShared _shared;
        //this role serves all this function
        private string requiredRole = ApplicationRoleNames.Collector;

        public RequestKeyService(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper, IShared shared)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _shared = shared;
        }

        public async Task<Message> CollectKey(Guid keyId, Activity activity, string userId)
        {
            var claimUser = await _shared.GetUser(userId, requiredRole);
            
            var initiateReturn = await InitiateReturn(claimUser);
                
            if ( initiateReturn is null)
            {
                var checkRoom = await _context.Key.Where(check => check.Id == keyId && check.Status == KeyStatus.Available).FirstOrDefaultAsync();
                if (checkRoom == null)
                {
                    throw new Exception("Key not available");
                }
                else
                {
                    var addRequest = new RequestKey
                    {
                        Id = Guid.NewGuid(),
                        Activity = activity,
                        Availability = CheckWith.InBoard,
                        _Key = checkRoom.Room,
                        CollectionTime = DateTime.UtcNow,
                        Status = Status.Pending,
                        KeyCollectorId = claimUser.Id,
                        Key = checkRoom,
                        GetKeyId = checkRoom.Id
                    };
                    await _context.RequestKey.AddAsync(addRequest);

                    checkRoom.Status = KeyStatus.PendingAcceptance;

                    await _context.SaveChangesAsync();

                    return new Message("Key request sent to the worker, await your reponse to take key");
                }
            }
            else
            {
                throw new Exception("You have an existing key with you, you will have to return it before you have access to get a new key");
            }
            
        }


        public async Task<Message> ReturnKey(string userId)
        {
            var claimUser = await _shared.GetUser(userId, requiredRole);

            var initiateReturn = await _context.RequestKey
                .Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();

            if (initiateReturn != null)
            {
                initiateReturn.ReturnedTime = DateTime.UtcNow;
                initiateReturn.Status = Status.CheckReturn;

                await _context.SaveChangesAsync();

                return new Message("Key return updated, waiting for worker to accept your request");
            }

            else
            {
                return new Message("unable to return key");
            }
        }

        public async Task<ViewUsage> GetView(string userId)
        {
            var claimUser = await _shared.GetUser(userId, requiredRole);

            var getKey = await InitiateReturn(claimUser);

            if (getKey != null)
            {
                return new ViewUsage{ RoomNumber = getKey._Key,  CollectionTime = getKey.CollectionTime, Activity = getKey.Activity, Status = getKey.Status};
            }

            else
            {
                throw new Exception("You don't have a key in hand at the moment");
            }
            
        }

        private async Task<RequestKey> InitiateReturn(User claimUser)
        {
            var getKey = await _context.RequestKey
                .Where(checkOne => checkOne.KeyCollectorId == claimUser.Id &&
                (checkOne.Availability == CheckWith.InHand || checkOne.Status == Status.Pending)).FirstOrDefaultAsync();

            return getKey;
        }
    }
}
