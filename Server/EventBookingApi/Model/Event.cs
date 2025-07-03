using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;


public class Event
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public EventType EventType { get; set; }

    public EventStatus EventStatus { get; set; } = EventStatus.Active;

    public EventCategory Category { get; set; }  

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid? ManagerId { get; set; }

    public User? Manager { get; set; }

    public Guid CityId { get; set; }                     
    public Cities City { get; set; } = null!;             

    public ICollection<TicketType>? TicketTypes { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
    public ICollection<BookedSeat>? BookedSeats { get; set; }
    public ICollection<EventImage>? Images { get; set; }
}
