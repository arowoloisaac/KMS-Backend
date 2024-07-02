
using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.DTOs;
using Key_Management_System.DTOs.AssignDto;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Key_Management_System.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Key_Management_System.Services.AssignKeyService
{
    public class AssignKeyService : IAssignKeyService
    {
        private readonly UserManager<User> _workerManager;
        private readonly ApplicationDbContext _context;
        private readonly IShared _shared;

        private string requiredRole = ApplicationRoleNames.Manager;

        public AssignKeyService(UserManager<User> workerManager, ApplicationDbContext context, IShared shared)
        {
            _workerManager = workerManager;
            _context = context;
            _shared = shared;
        }

        public async Task<Message> AssignCollectorKey(Guid keyId, General check, string workerId)
        {
            var claimUser = await _shared.GetUser(workerId, requiredRole);

            var checkRoom = await _context.Key.FirstOrDefaultAsync(check => check.Id == keyId && check.Status == KeyStatus.PendingAcceptance);

            if (checkRoom == null)
            {
                return new Message("Such room does not exist in the database");
            }
            else
            {
                var checkRequest = await _context.RequestKey
                    .FirstOrDefaultAsync(check => check._Key == checkRoom.Room && check.Status == Status.Pending && check.KeyCollectorId != claimUser.Id);
                if ( checkRequest != null)
                {
                    if (check == General.Accept)
                    {
                        checkRequest.Status = Status.Accept;
                        checkRequest.Availability = CheckWith.InHand;
                        checkRequest.AssignedTime = DateTime.UtcNow;
                        checkRoom.Status = KeyStatus.Unavailable;
                    }

                    else if(check == General.Decline)
                    {
                        checkRequest.Status = Status.Decline;
                        checkRequest.Availability = CheckWith.InBoard;

                        checkRoom.Status = KeyStatus.Available;
                    }
                    checkRequest.GetWorkerId = claimUser.Id;
                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return new Message("You can't perform this task, you can't be the collector and assignee");
                }
                return new Message("Task Successful");
            }
        }

        public async Task<Message> AcceptKeyReturn(Guid keyId, General check, string workerId)
        {
            var claimUser = await _shared.GetUser(workerId, requiredRole);

            var checkRoom = await _context.Key.Where(check => check.Id == keyId && check.Status == KeyStatus.Unavailable).FirstOrDefaultAsync();

            if (checkRoom == null)
            {
                return new Message("room is not available");
            }
            else
            {
                var checkRequest = await _context.RequestKey.FirstOrDefaultAsync
                    (check => (check._Key == checkRoom.Room && check.Status == Status.Accept || check.Status == Status.ThirdParty) && check.KeyCollectorId != claimUser.Id);
                if (checkRequest != null )
                {
                    if (check == General.Accept)
                    {
                        checkRequest.Status = Status.AcceptSignOut;
                        checkRequest.Availability = CheckWith.InBoard;
                        checkRequest.ReturnedTime = DateTime.UtcNow;
                        checkRoom.Status = KeyStatus.Available;
                    }

                    else if (check == General.Decline)
                    {
                        checkRequest.Status = Status.DeclineSignOut;

                        checkRoom.Status = KeyStatus.Unavailable;
                    }
                    checkRequest.GetWorkerId = claimUser.Id;
                    
                    await _context.SaveChangesAsync();
                }
                return new Message("key now in the database");
            }
        }

        public async Task<List<KeyCollectorRequest>> CheckRequest()
        {
            var requestResponse = await _context.RequestKey.Where(filter => filter.Status == Status.Pending).ToListAsync();

            if (requestResponse == null)
            {
                return new List<KeyCollectorRequest>();
            }

            var responseList = requestResponse.Select(requestResponse => new KeyCollectorRequest
            {
                KeyId = requestResponse.GetKeyId,
                Room = requestResponse._Key
            }).ToList();

            return responseList;
        }

        public async Task<List<KeyCollectorRequest>> CheckReturns()
        {
            var requestResponses = await _context.RequestKey.Where(filter => filter.Status == Status.CheckReturn).ToListAsync();

            if (requestResponses == null)
            {
                return new List<KeyCollectorRequest>();
            }

            var getReponse = requestResponses.Select(requestResponses => new KeyCollectorRequest
            {
                KeyId = requestResponses.GetKeyId,
                Room = requestResponses._Key,
            }).ToList();
            return getReponse;
        }
    }
}
