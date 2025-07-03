using System;

namespace EventBookingApi.Model.DTO;

public class TicketResponseDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string TicketType { get; set; } = string.Empty;
    public decimal PricePerTicket { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => PricePerTicket * Quantity;
    public DateTime? BookedAt { get; set; }

    public List<int>? SeatNumbers { get; set; }

    public PaymentResponseDTO? Payment { get; set; }
}
