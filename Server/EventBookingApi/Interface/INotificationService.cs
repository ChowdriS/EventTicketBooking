using System;

namespace EventBookingApi.Interface
{
    public interface INotificationService
    {
        public Task NotifyUser(Guid userId, string message, string notificationType);
        public Task NotifyAdmins(string message, string notificationType);
        public Task NotifyEventManagers(Guid eventId, string message, string notificationType);
    }
}