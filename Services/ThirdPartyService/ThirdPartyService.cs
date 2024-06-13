using Key_Management_System.Data;
using Key_Management_System.DTOs.ThirdPartyDto;
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

            

            if (checkUser == null)
            {
                return new Message("You have to login or register to the account to perform such task");
            }

            else
            {
                var initiateReturn = await _context.RequestKey.Where(check => check.KeyCollectorId == checkUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();

                var checkRoom = await _context.Key.FirstOrDefaultAsync(check => check.Id == keyId && check.Status == KeyStatus.Unavailable);

                var checkAwaitingRequest = await _context.ThirdParty.Where(check => check.KeyId == keyId && check.Request == TPRequest.Pending).FirstOrDefaultAsync();    

                if (initiateReturn is null && checkRoom is not null && checkAwaitingRequest is null)
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
                var validateHolder = await 
                    _context.RequestKey.Where(validate => validate.KeyCollectorId == currentHolder.Id && validate.Availability == CheckWith.InHand)
                    .FirstOrDefaultAsync();

                if (validateHolder is not null)
                {
                    var thirdPartyRequest = await 
                        _context.ThirdParty.FirstOrDefaultAsync(check => check.KeyId == validateHolder.GetKeyId && check.Request == TPRequest.Pending);

                    if (thirdPartyRequest is not null)
                    {
                        thirdPartyRequest.CurrentHolder = currentHolder.Id;
                        thirdPartyRequest.Request = TPRequest.Accept;
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
        }

        public async Task<ThirdPartyRequest> GetRequest(string currentHolder)
        {
            var claimUser = await _userManager.FindByIdAsync(currentHolder);

            if (claimUser == null)
            {
                throw new Exception("not logged in");
            }

            else
            {
                var request = await 
                    _context.RequestKey.Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();

                var getKey = await _context.Key.Where(find => find.Status == KeyStatus.Unavailable && find.Id == request.GetKeyId).FirstOrDefaultAsync();

                if (request == null && getKey == null)
                {
                    throw new Exception("no key");
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
            throw new NotImplementedException();
        }

        public async Task<bool> Notifier(string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            if (claimUser == null)
            {
                throw new Exception("null");
            }
            /*if (claimUser == null)
            {
                throw new Exception("User not logged in");
            }*/

            else
            {
                var request = await _context.RequestKey.FirstOrDefaultAsync(checkUser => checkUser.KeyCollectorId == claimUser.Id && checkUser.Availability == CheckWith.InHand);
                if (request == null)
                {
                    return false;
                }


                else
                {
                    var checkRequest = await _context.ThirdParty.Where(check => check.KeyId == request.GetKeyId).FirstOrDefaultAsync();
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

                
                if (validateHolder is not null )
                {
                    var thirdPartyRequest = await 
                        _context.ThirdParty.FirstOrDefaultAsync(check => check.KeyId == validateHolder.GetKeyId && check.Request == TPRequest.Pending);
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
                        throw new Exception("Check the key and validation");
                    }
                    
                }

                else
                {
                    throw new Exception("You can't perform this task due to error");
                }
            }
        }

        
    }
}
