using Freelify.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Freelify.Hubs
{
    [Authorize]

    public class NotificationHub : Hub
    {

       
    }
}
