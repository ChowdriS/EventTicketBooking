using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;

public class Ticket
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid EventId { get; set; }
    public Event? Event { get; set; }
    public Guid TicketTypeId { get; set; }
    public TicketType? TicketType { get; set; }
    public int BookedQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Booked;
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public Guid? PaymentId { get; set; }
    public Payment? Payment { get; set; }
    
    public ICollection<BookedSeat>? BookedSeats { get; set; }
}

