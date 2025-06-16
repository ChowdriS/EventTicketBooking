using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;
namespace EventBookingApi.Misc;

public class NotificationHub : Hub
{
     public async Task JoinGroup(string groupName)
    {
        System.Console.WriteLine(groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}