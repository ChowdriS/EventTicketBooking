using System;

namespace EventBookingApi.Model.DTO;

public class EventResponseTicketTypeDTO
{
    public Guid Id { get; set; } 
    public TicketTypeEnum TypeName { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public int BookedQuantity { get; set; } = 0;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;
}
