using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;

public class BookedSeat
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid EventId { get; set; }
    public Event? Event { get; set; }

    [Required]
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public int SeatNumber { get; set; }

    public BookedSeatStatus BookedSeatStatus { get; set; } = BookedSeatStatus.Booked;

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
}

