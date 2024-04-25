using AutoMapper;
using Key_Management_System.Data;
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

        public async Task<Message> CollectKey(string key, Activity activity, string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            var checkRoom =  await _context.Key.Where(check => check.Room == key && check.Status == KeyStatus.Available).FirstOrDefaultAsync();

            
            if (claimUser == null)
            {
                return new Message("No user is logged in");
            }

            else
            {
                var initiateReturn = await _context.RequestKey.Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();
                if ( initiateReturn is null && checkRoom is not null )
                {
                    var addRequest = new RequestKey
                    {
                        Id = Guid.NewGuid(),
                        Activity = activity,
                        Availability = CheckWith.InHand,
                        _Key = key,
                        CollectionTime = DateTime.UtcNow,
                        Status = Status.Pending,
                        KeyCollectorId = claimUser.Id,
                        Key = checkRoom,
                    };
                    await _context.RequestKey.AddAsync(addRequest);

                    checkRoom.Status = KeyStatus.PendingAcceptance;
                    
                    await _context.SaveChangesAsync();

                    return new Message("Key request sent to the worker, await your reponse to take key");
                }

                else
                {
                    return new Message("You have an existing key with you, you will have to return it before you have access to get a now key");
                }
            }
        }

        public async Task<Message> ReturnKey(string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            var initiateReturn = await _context.RequestKey.Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();

            if (claimUser == null)
            {
                return new Message("There is no user logged in");
            }

            else
            {
                if (initiateReturn != null)
                {
                    initiateReturn.ReturnedTime = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return new Message("Key return updated, waiting for worker to accept your request");
                }

                else
                {
                    return new Message("unable to return key");
                }
            }
        }
    }
}
