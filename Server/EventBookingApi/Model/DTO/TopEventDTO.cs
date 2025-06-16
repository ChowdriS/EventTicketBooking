using System;

namespace EventBookingApi.Model.DTO;

public class TopEventDTO
{
    public Guid EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
}