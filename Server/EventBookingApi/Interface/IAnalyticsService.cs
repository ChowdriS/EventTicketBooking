using System;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;
public interface IAnalyticsService
{
    Task<decimal> GetTotalEarnings(Guid managerId);
    Task<List<BookingTrendDTO>> GetBookingTrends();
    Task<List<TopEventDTO>> GetTopEvents();
}
