using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model.DTO;

public class TicketBookRequestDTO
{
    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid TicketTypeId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    public List<int>? SeatNumbers { get; set; }
    [Required]
    public PaymentRequestDTO? Payment { get; set; }
}
