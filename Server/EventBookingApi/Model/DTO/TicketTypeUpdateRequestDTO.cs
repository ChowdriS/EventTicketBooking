using System;

namespace EventBookingApi.Model.DTO;

public class TicketTypeUpdateRequestDTO
{
public decimal? Price { get; set; }
    public int? TotalQuantity { get; set; }
    public string? Description { get; set; }

}
