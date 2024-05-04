﻿using Key_Management_System.Enums;
using Key_Management_System.Models;

namespace Key_Management_System.Services.ThirdPartyService
{
    public interface IThirdPartyService
    {
        Task<Message> AcceptRequest(Guid keyId, string currentUser);

        Task<Message> RejectRequest(Guid keyId, string currentUser);

        Task GetRequest();

        //Task<bool> Notifier(string userId);

        Task<Message> SendRequest(Guid keyId, Activity activity, string userId);
    }
}
