using AutoMapper;
using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.DTOs;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Enums;
using Key_Management_System.Models;
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

        public RequestKeyService(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Message> CollectKey(Guid keyId, Activity activity, string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            if (claimUser == null || !await _userManager.IsInRoleAsync(claimUser, ApplicationRoleNames.Collector))
            {
                throw new Exception("No user is logged in or User does not have the correct role");
            }

            else
            {
                var initiateReturn = await _context.RequestKey
                    .Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand || check.Status == Status.Pending).FirstOrDefaultAsync();
                
                if ( initiateReturn is null)
                {
                    var checkRoom = await _context.Key.Where(check => check.Id == keyId && check.Status == KeyStatus.Available).FirstOrDefaultAsync();
                    if (checkRoom is not null)
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
                    else
                    {
                        throw new Exception("Key not available");
                    }
                }
                else
                {
                    throw new Exception("You have an existing key with you, you will have to return it before you have access to get a new key");
                }
            }
        }

        public async Task<Message> ReturnKey(string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            if (claimUser == null)
            {
                return new Message("There is no user logged in");
            }

            else
            {
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
        }

        public async Task<ViewUsage> GetView(string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            if (claimUser == null)
            {
                throw new Exception("User is not logged in or registered");
            }

            else
            {
                var getKey = await _context.RequestKey
                    .FirstOrDefaultAsync(checkOne => checkOne.KeyCollectorId == claimUser.Id && checkOne.Availability == CheckWith.InHand || checkOne.Status == Status.Pending);

                if (getKey != null)
                {
                    return new ViewUsage{ RoomNumber = getKey._Key, CollectionTime = getKey.CollectionTime, Activity = getKey.Activity, Status = getKey.Status};
                }

                else
                {
                    throw new Exception("You don't have a key in hand at the moment");
                }
            }
        }
    }
}
