
using Key_Management_System.Data;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task AssignCollectorKey(string key, General check, string workerId)
        {
            var claimUser = await _workerManager.FindByIdAsync(workerId);

            var checkRequest = await _context.RequestKey.FirstOrDefaultAsync(check => check._Key == key && check.Status == Status.Pending);

            var updateRoom = await _context.Key.FirstOrDefaultAsync(check => check.Room == key && check.Status == KeyStatus.PendingAcceptance);

            if (claimUser == null)
            {
                throw new Exception("User must be logged in");
            }

            else
            {
                if (claimUser is Worker worker && checkRequest != null)
                {
                    if (check == General.Accept)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.Accept;
                        //checkRequest.Availability == CheckWith.InBoard;

                        updateRoom.Status = KeyStatus.Unavailable;
                    }

                    else if(check == General.Decline)
                    {
                        checkRequest.Worker = worker;
                        checkRequest.Status = Status.Decline;

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
                    throw new Exception("You can't perform this task");
                }
            }
        }

        //this funnction has an enum which prompts the worker to either accept or 
        public async Task AcceptKeyReturn(string key, General check, string workerId)
        {
            var claimUser = await _workerManager.FindByIdAsync(workerId);

            var checkRequest = await _context.RequestKey.FirstOrDefaultAsync(check => check._Key == key && check.Status == Status.Accept);

            var updateRoom = await _context.Key.Where(check => check.Room == key && check.Status == KeyStatus.Unavailable).FirstOrDefaultAsync();

            if (claimUser == null)
            {
                throw new Exception("User not logged in or registered");
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
                        throw new Exception("Unale to perform this task");
                    }

                    await _context.SaveChangesAsync();
                }

            }
        }
    }
}
