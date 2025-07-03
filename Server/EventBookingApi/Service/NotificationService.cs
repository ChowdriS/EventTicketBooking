using System;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using Microsoft.AspNetCore.SignalR;

namespace EventBookingApi.Service;

public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Event> _eventRepository;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            IRepository<Guid, User> userRepository,
            IRepository<Guid, Event> eventRepository)
        {
            _hubContext = hubContext;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }
        public async Task NotifyUser(Guid userId, string message, string notificationType)
        {
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notificationType,message);
        }

        public async Task NotifyAdmins(string message, string notificationType)
        {
            await _hubContext.Clients.Group("admins").SendAsync("ReceiveNotification", notificationType, message);
        }

        public async Task NotifyEventManagers(Guid eventId, string message, string notificationType)
        {
            var eventObj = await _eventRepository.GetById(eventId);
            if (eventObj?.ManagerId != null)
            {
                await _hubContext.Clients.Group($"manager_{eventObj.ManagerId}")
                    .SendAsync("ReceiveNotification", notificationType, message);
            }
        }
    }