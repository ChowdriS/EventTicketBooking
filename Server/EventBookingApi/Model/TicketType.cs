using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;

public class TicketType
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EventId { get; set; }
    public Event? Event { get; set; }

    [Required]
    public TicketTypeEnum TypeName { get; set; } = TicketTypeEnum.Regular;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int TotalQuantity { get; set; }

    public int BookedQuantity { get; set; } = 0;

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}
