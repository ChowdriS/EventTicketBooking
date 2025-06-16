using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model.DTO;

public class TicketTypeAddRequestDTO
{
    public Guid EventId { get; set; }
    public TicketTypeEnum TypeName { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public string? Description { get; set; }
}
