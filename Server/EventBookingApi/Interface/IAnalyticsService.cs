using System;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;
public interface IAnalyticsService
{
    Task<Dictionary<string, decimal>> GetTotalEarnings(Guid managerId);
    Task<List<BookingTrendDTO>> GetBookingTrends();
    Task<List<TopEventDTO>> GetTopEvents();
}
