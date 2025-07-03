using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model.DTO;

public class EventAddRequestDTO
{
    [Required]
    public string? Title { get; set; }
    public string? Description { get; set; }
    [Required]
    public DateTime EventDate { get; set; }
    [Required]
    public EventType EventType { get; set; }
    public EventCategory Category { get; set; }
    public Guid CityId { get; set; }
    public List<TicketTypeAddRequestDTO>? TicketTypes { get; set; }
    public IFormFile? Image { get; set; }
}
