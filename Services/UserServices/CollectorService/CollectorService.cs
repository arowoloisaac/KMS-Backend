using Key_Management_System.DTOs.UserDto.KeyCollectorDto;
using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Key_Management_System.Services.UserServices.CollectorService
{
    public class CollectorService : ICollectorService
    {
        private readonly UserManager<User> _collectorManager;

        public CollectorService(UserManager<User> collectorManager)
        {
            _collectorManager = collectorManager;
        }


        public async Task RegisterCollector(RegisterCollectorDto collectorDto)
        {
            var existingUser = await _collectorManager.FindByEmailAsync(collectorDto.Email);



            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {existingUser.Email} already exist");
            }

            else
            {
                var createUser = await _collectorManager.CreateAsync(new KeyCollector
                {
                    Name = collectorDto.Name,
                    Email = collectorDto.Email,
                    PhoneNumber = collectorDto.PhoneNumber,
                    UserName = collectorDto.Name,
                    Password = collectorDto.Password,
                }, collectorDto.Password);

                if (!createUser.Succeeded)
                {
                    throw new Exception("Unable to create account with those credentials");
                }
            }
        }
    }
}
