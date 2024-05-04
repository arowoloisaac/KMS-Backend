using Key_Management_System.Data;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Key_Management_System.Services.ThirdPartyService
{
    public class ThirdPartyService : IThirdPartyService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ThirdPartyService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Message> SendRequest(Guid keyId, Activity activity, string userId)
        {
            var checkUser = await _userManager.FindByIdAsync(userId);

            var checkRoom = await _context.Key.FirstOrDefaultAsync(check => check.Id == keyId && check.Status == KeyStatus.Unavailable);

            if (checkUser == null)
            {
                return new Message("You have tp login or register to the account to perform such task");
            }

            else
            {
                var initiateReturn = await _context.RequestKey.Where(check => check.KeyCollectorId == checkUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();
                if (initiateReturn is null && checkRoom is not null)
                {
                    var addRequest = new ThirdParty
                    {
                        Id = Guid.NewGuid(),
                        KeyId = keyId,
                        KeyCollectorId = checkUser.Id,
                        General = General.Pending,
                        Activity = activity,
                    };

                    await _context.ThirdParty.AddAsync(addRequest);

                    await _context.SaveChangesAsync();

                    return new Message("Your request for third party key has been sent await your response");
                }

                else
                {
                    return new Message("You have an existing key with you, you will have to return it before you have access to get a new key");
                }
            }
        }


        public async Task<Message> AcceptRequest(Guid keyId, string userId)
        {
            var currentHolder = await _userManager.FindByIdAsync(userId);

            if (currentHolder is null)
            {
                return new Message("User not either logged in or registered");
            }

            else
            {
                var validateHolder = await _context.RequestKey.Where(validate => validate.KeyCollectorId == currentHolder.Id && validate.Availability == CheckWith.InHand)
                    .FirstOrDefaultAsync();

                var thirdPartyRequest = await _context.ThirdParty.FirstOrDefaultAsync(check => check.KeyId == keyId && check.General == General.Pending);

                if (validateHolder is not null && thirdPartyRequest is not null)
                {
                    thirdPartyRequest.CurrentHolder = currentHolder.Id;
                    thirdPartyRequest.General = General.Accept;
                    thirdPartyRequest.RequestKey = validateHolder;

                    //change the status to third party in order for the second collector to have access
                    validateHolder.Status = Status.ThirdParty;
                    validateHolder.ReturnedTime = DateTime.UtcNow;
                    validateHolder.Availability = CheckWith.ThirdParty;

                    var addNewRequest = new RequestKey
                    {
                        Id = Guid.NewGuid(),
                        KeyCollectorId = thirdPartyRequest.KeyCollectorId,
                        Activity = thirdPartyRequest.Activity,
                        _Key = validateHolder._Key,
                        Key = validateHolder.Key,
                        Availability = CheckWith.InHand,
                        CollectionTime = DateTime.UtcNow,
                        Status = Status.ThirdParty,
                    };

                    await _context.SaveChangesAsync();

                    return new Message("Request accepted");
                }

                else
                {
                    return new Message("You just can't perform this task");
                }
            }
        }

        public Task GetRequest()
        {
            throw new NotImplementedException();
        }

        public async Task<Message> RejectRequest(Guid keyId, string currentUser)
        {
            var currentHolder = await _userManager.FindByIdAsync(currentUser);

            if (currentHolder is null)
            {
                return new Message("User either no logged in or registered");
            }

            else
            {
                var validateHolder = await _context.RequestKey.Where(validate => validate.KeyCollectorId == currentHolder.Id && validate.Availability == CheckWith.InHand)
                    .FirstOrDefaultAsync();

                var thirdPartyRequest = await _context.ThirdParty.FirstOrDefaultAsync(check => check.KeyId == keyId && check.General == General.Pending);

                if (validateHolder is not null && thirdPartyRequest is not null)
                {
                    thirdPartyRequest.CurrentHolder = currentHolder.Id;
                    thirdPartyRequest.General = General.Decline;
                    thirdPartyRequest.RequestKey = validateHolder;

                    await _context.SaveChangesAsync();

                    return new Message("Collection rejected by current holder");
                }

                else
                {
                    return new Message("You can't perform this task");
                }
            }
        }

        
    }
}
