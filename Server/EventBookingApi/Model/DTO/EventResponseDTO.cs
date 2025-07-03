using System;

namespace EventBookingApi.Model.DTO;

public class EventResponseDTO
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public string? Category { get; set; }

    public string? Location { get; set; }
    public string? EventStatus { get; set; }
    public string? EventType { get; set; }
    public ICollection<EventResponseTicketTypeDTO>? TicketTypes { get; set; }
    public ICollection<EventResponseBookedSeatDTO>? BookedSeats { get; set; }
    public ICollection<Guid>? Images { get; set; }

}