
using Key_Management_System.Data;
using Key_Management_System.DTOs;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Key_Management_System.Services.AssignKeyService
{
    public class AssignKeyService : IAssignKeyService
    {
        private readonly UserManager<User> _workerManager;
        private readonly ApplicationDbContext _context;

        public AssignKeyService(UserManager<User> workerManager, ApplicationDbContext context)
        {
            _workerManager = workerManager;
            _context = context;
        }

        public async Task<Message> AssignCollectorKey(Guid keyId, General check, string workerId)
        {
            var claimUser = await _workerManager.FindByIdAsync(workerId);

            var updateRoom = await _context.Key.FirstOrDefaultAsync(check => check.Id == keyId && check.Status == KeyStatus.PendingAcceptance);

            if (updateRoom == null)
            {
                return new Message("room is not available");
            }


            var checkRequest = await _context.RequestKey.FirstOrDefaultAsync(check => check._Key == updateRoom.Room && check.Status == Status.Pending);

            if (claimUser == null)
            {
                return new Message("User must be logged in");
            }

            else
            {
                if (claimUser is Worker worker && checkRequest != null)
                {
                    if (check == General.Accept)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.Accept;
                        checkRequest.Availability = CheckWith.InHand;

                        updateRoom.Status = KeyStatus.Unavailable;
                    }

                    else if(check == General.Decline)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.Decline;
                        checkRequest.Availability = CheckWith.InBoard;

                        updateRoom.Status = KeyStatus.Available;
                    }

                    else
                    {
                        throw new Exception("Unable to perform task");
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    return new Message("You can't perform this task");
                }
                return new Message("Task Successful");
            }
        }

        //this funnction has an enum which prompts the worker to either accept or 
        public async Task<Message> AcceptKeyReturn(Guid keyId, General check, string workerId)
        {
            var claimUser = await _workerManager.FindByIdAsync(workerId);

            var updateRoom = await _context.Key.Where(check => check.Id == keyId && check.Status == KeyStatus.Unavailable).FirstOrDefaultAsync();

            if (updateRoom == null)
            {
                return new Message("room is not available");
            }

            var checkRequest = await _context.RequestKey.FirstOrDefaultAsync(check => check._Key == updateRoom.Room && check.Status == Status.Accept || check.Status ==Status.ThirdParty);

            

            if (claimUser == null)
            {
                return new Message("User not logged in or registered");
            }

            else
            {
                if( claimUser is Worker worker && checkRequest != null )
                {
                    if (check == General.Accept)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.AcceptSignOut;
                        checkRequest.Availability = CheckWith.InBoard;

                        updateRoom.Status = KeyStatus.Available;
                    }

                    else if (check == General.Decline)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.DeclineSignOut;

                        updateRoom.Status = KeyStatus.Unavailable;
                    }

                    else
                    {
                        return new Message("Unale to perform this task");
                    }

                    await _context.SaveChangesAsync();
                }
                return new Message("Task succeeded");
            }
        }

        public async Task<List<KeyWith>> CheckRequest()
        {
            var classRoom = await _context.RequestKey.Where(filter => filter.Status == Status.Pending).FirstOrDefaultAsync();

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
            };

            return new List<KeyWith> { getReponse };
        }
    }
}
