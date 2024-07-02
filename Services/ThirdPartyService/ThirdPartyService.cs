using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.DTOs.ThirdPartyDto;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Key_Management_System.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Key_Management_System.Services.ThirdPartyService
{
    public class ThirdPartyService : IThirdPartyService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IShared _shared;

        private string requiredRole = ApplicationRoleNames.Collector;

        public ThirdPartyService(UserManager<User> userManager, ApplicationDbContext context, IShared shared)
        {
            _userManager = userManager;
            _context = context;
            _shared = shared;
        }

        public async Task<Message> SendRequest(Guid keyId, Activity activity, string userId)
        {
            var checkUser = await _shared.GetUser(userId, requiredRole);

            var initiateReturn = await InitiateReturn(checkUser);
                
            var checkRoom = await _context.Key.FirstOrDefaultAsync(check => check.Id == keyId && check.Status == KeyStatus.Unavailable);
            if (checkRoom is null)
            {
                throw new Exception("key is not available in the database");
            }

            var checkAwaitingRequest = await _context.ThirdParty
                .Where(check => check.KeyId == checkRoom.Id && 
                check.Request == TPRequest.Pending).FirstOrDefaultAsync();    

            if (initiateReturn is null && checkAwaitingRequest is null)
            {
                var addRequest = new ThirdParty
                {
                    Id = Guid.NewGuid(),
                    KeyId = keyId,
                    KeyCollectorId = checkUser.Id,
                    Request = TPRequest.Pending,
                    Activity = activity,
                };

                await _context.ThirdParty.AddAsync(addRequest);
                await _context.SaveChangesAsync();

                return new Message("Your request for third party key has been sent await your response");
            }
            else
            {
                throw new Exception("You have an existing key with you, you will have to return it before you have access to get a new key");
            }
        }

        public async Task<Message> AcceptRequest(Guid keyId, string userId)
        {
            var currentHolder = await _shared.GetUser(userId, requiredRole);

            var validateHolder = await InitiateReturn(currentHolder);

            var checkRoom = await _context.Key.Where(check => check.Id == keyId).FirstOrDefaultAsync();

            if (validateHolder is not null)
            {
                var thirdPartyRequest = await 
                    _context.ThirdParty.FirstOrDefaultAsync
                    (check => check.KeyId == validateHolder.GetKeyId
                    && check.Request == TPRequest.Pending);

                if (thirdPartyRequest is not null)
                {
                    thirdPartyRequest.CurrentHolder = currentHolder.Id;
                    thirdPartyRequest.Request = TPRequest.Accept;
                    thirdPartyRequest.RequestKey = validateHolder;

                    validateHolder.Status = Status.ThirdParty;
                    validateHolder.ReturnedTime = DateTime.UtcNow;
                    validateHolder.Availability = CheckWith.ThirdParty;

                    var addNewRequest = new RequestKey
                    {
                        Id = Guid.NewGuid(),
                        KeyCollectorId = thirdPartyRequest.KeyCollectorId,
                        Activity = thirdPartyRequest.Activity,
                        _Key = validateHolder._Key,
                        Key = checkRoom,
                        Availability = CheckWith.InHand,
                        CollectionTime = DateTime.UtcNow,
                        AssignedTime = DateTime.UtcNow,
                        Status = Status.ThirdParty,
                        GetKeyId = validateHolder.GetKeyId,
                    };
                    await _context.RequestKey.AddAsync(addNewRequest);
                    await _context.SaveChangesAsync();

                    return new Message("Request accepted");
                }
                else
                {
                    throw new Exception("Can't validate user or check key");
                }
            }
            else
            {
                throw new Exception("You just can't perform this task");
            }
        }

        public async Task<ThirdPartyRequest> GetRequest(string currentHolder)
        {
            var claimUser = await _shared.GetUser(currentHolder, requiredRole);

            var request = await InitiateReturn(claimUser);

            var getKey = await _context.Key.Where(find => find.Status == KeyStatus.Unavailable && find.Id == request.GetKeyId).FirstOrDefaultAsync();

            if (request == null && getKey == null)
            {
                throw new Exception("No key request from a third party");
            }
            else
            {
                var checkTp = await _context.ThirdParty.FirstOrDefaultAsync(search => search.KeyId == getKey.Id);

                if (checkTp == null)
                {
                    return null;
                }
                return new ThirdPartyRequest { Id = checkTp.Id, KeyId = checkTp.Id, Name = getKey.Room, Activity = checkTp.Activity };
            }
            
        }

        public async Task<bool> Notifier(string userId)
        {
            var claimUser = await _shared.GetUser(userId, requiredRole);

            var request = await InitiateReturn(claimUser);
            if (request == null)
            {
                return false;
            }

            else
            {
                var checkRequest = await _context.ThirdParty
                    .Where(check => check.KeyId == request.GetKeyId).FirstOrDefaultAsync();
                if (checkRequest == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<Message> RejectRequest(Guid keyId, string currentUser)
        {
            var currentHolder = await _shared.GetUser(currentUser, requiredRole);

            var validateHolder = await InitiateReturn(currentHolder);

            if (validateHolder is not null )
            {
                var thirdPartyRequest = await _context.ThirdParty
                    .FirstOrDefaultAsync(check => check.KeyId == validateHolder.GetKeyId && check.Request == TPRequest.Pending);
                if (thirdPartyRequest is not null)
                {
                    thirdPartyRequest.CurrentHolder = currentHolder.Id;
                    thirdPartyRequest.Request = TPRequest.Decline;
                    thirdPartyRequest.RequestKey = validateHolder;

                    await _context.SaveChangesAsync();

                    return new Message("Collection rejected by current holder");
                }
                else
                {
                    throw new Exception("Unable to perform this task");
                }
            }
            else
            {
                throw new Exception("You can't perform this task due to user not validated");
            }
        }

        private async Task<RequestKey> InitiateReturn(User claimUser)
        {
            var getKey = await _context.RequestKey
                .Where(find => find.KeyCollectorId == claimUser.Id &&
                find.Availability == CheckWith.InHand).FirstOrDefaultAsync();

            return getKey;
        }
    }
}
