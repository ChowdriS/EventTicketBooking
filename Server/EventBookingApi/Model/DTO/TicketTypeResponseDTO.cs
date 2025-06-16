using System;

namespace EventBookingApi.Model.DTO;

public class TicketTypeResponseDTO
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public TicketTypeEnum TypeName { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public int BookedQuantity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
