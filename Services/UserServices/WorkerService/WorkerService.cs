using Key_Management_System.DTOs.UserDto.WorkerDto;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Key_Management_System.Services.UserServices.WorkerService
{
    public class WorkerService : IWorkerService
    {
        private UserManager<User> _workerManager;

        public WorkerService(UserManager<User> workerManager)
        {
            _workerManager = workerManager;
        }


        public async Task RegisterWorker(RegisterWorkerDto workerDto)
        {
            var existingUser = await _workerManager.FindByEmailAsync(workerDto.Email);

            var checkAdminExistence = await _workerManager.FindByEmailAsync("admin@example.com");


            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {existingUser.Email} already exist");
            }

            else
            {
                var createUser = await _workerManager.CreateAsync(new Worker
                {
                    Name = workerDto.Name,
                    Email = workerDto.Email,
                    PhoneNumber = workerDto.PhoneNumber,
                    Faculty = workerDto.Faculty,
                    UserName = workerDto.Name,
                    Password = workerDto.Password,
                }, workerDto.Password);

                if (!createUser.Succeeded)
                {
                    throw new Exception("Unable to create account with those credentials");
                }
            }

            if (checkAdminExistence == null)
            {
                var AdminUser = new Worker
                {
                    Name = "Administrator",
                    Email = "admin@gmail.com",
                    UserName = "Administrator",
                    Password = "example123"
                };

                var result = await _workerManager.CreateAsync(AdminUser, "example123");

                if (!result.Succeeded)
                {
                    throw new Exception("Unable to create user admin");
                }
            }
        }
    }
}
