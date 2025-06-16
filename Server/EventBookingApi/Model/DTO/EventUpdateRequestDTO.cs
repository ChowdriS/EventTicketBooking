using System;

namespace EventBookingApi.Model.DTO;

public class EventUpdateRequestDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public DateTime? EventDate { get; set; }
    public EventType? EventType { get; set; }

    public EventStatus? EventStatus { get; set; }
}
