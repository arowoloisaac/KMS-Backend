using AutoMapper;
using Azure;
using Key_Management_System.Configuration;
using Key_Management_System.DTOs.AuthenticationDto;
using Key_Management_System.Enums;
using Key_Management_System.Models;
using Key_Management_System.Services.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Key_Management_System.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IShared _shared;

        private string requiredRole = ApplicationRoleNames.Admin;

        public AuthenticationService(UserManager<User> userManager, IShared shared)
        {
            _userManager = userManager;
            _shared = shared;
        }

        public async Task<Message> AddRole(Guid userId, Roles role, string adminId)
        {
            var adminUser = await _shared.GetUser(adminId, requiredRole);
  
            var validateUser = await _userManager.FindByIdAsync(userId.ToString());

            if (validateUser != null)
            {
                if (!await _userManager.IsInRoleAsync(validateUser, role.ToString()))
                {
                    var response = await _userManager.AddToRoleAsync(validateUser, role.ToString());

                    if (response.Succeeded)
                    {
                        return new Message($"role added for user: {validateUser.UserName} with the role: {role}");
                    }
                    else
                    {
                        return new Message($"Unable to add user: {validateUser.UserName} to the role {role}");
                    }
                }
                else
                {
                    return new Message($"User: {validateUser.Email} is in role already");
                }
            }
            else
            {
                throw new Exception($"can't find user {validateUser.Email} in the database");
            }
        }

        public async Task<Message> RemoveRole(Guid userId, Roles role, string adminId)
        {
            var adminUser = await _shared.GetUser(adminId, requiredRole);

            var validateUser = await _userManager.FindByIdAsync(userId.ToString());

            if (validateUser != null)
            {
                if (!await _userManager.IsInRoleAsync(validateUser, role.ToString()))
                {
                    return new Message($"User: {validateUser.UserName} is not in role: {role}");
                }

                else
                {
                    var response = await _userManager.RemoveFromRoleAsync(validateUser, role.ToString());

                    if (response.Succeeded)
                    {
                        return new Message($"User: {validateUser.UserName} has been removed from role: {role}");
                    }

                    else
                    {
                        return new Message($"Unable to remove user: {validateUser.UserName} from the role: {role}");
                    }
                }
            }

            else
            {
                throw new ArgumentNullException(" can't find user");
            }
        }

        public async Task<List<UsersDto>> Users()
        {
            var users = await _userManager.Users.Where(getEmails => getEmails.Email != null).ToListAsync();

            if (users == null)
            {
                throw new UnauthorizedAccessException("User is null");
            }

            var response = users.Select( users => new UsersDto 
            {
                Id = users.Id,
                FirstName = users.FirstName,
                LastName = users.LastName,
                Email = users.Email
            }).ToList();

            return  response;
        }

    }
}
