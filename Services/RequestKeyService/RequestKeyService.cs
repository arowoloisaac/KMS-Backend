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

        public async Task CollectKey(string key, Activity activity, string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            var checkRoom =  await _context.Key.Where(check => check.Room == key && check.Status == KeyStatus.Available).FirstOrDefaultAsync();

            
            if (claimUser == null)
            {
                throw new Exception("OOPS, you have to log in to perform this action");
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
                }

                else
                {
                    throw new Exception("You have an existing key");
                }
            }
        }

        public async Task ReturnKey(string userId)
        {
            var claimUser = await _userManager.FindByIdAsync(userId);

            var initiateReturn = await _context.RequestKey.Where(check => check.KeyCollectorId == claimUser.Id && check.Availability == CheckWith.InHand).FirstOrDefaultAsync();

            if (claimUser == null)
            {
                Console.WriteLine("OOPs, you have to login to perform such action");
            }

            else
            {
                if (initiateReturn != null)
                {
                    initiateReturn.ReturnedTime = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }

                else
                {
                    Console.WriteLine("unable to return key");
                }
            }
        }
    }
}
