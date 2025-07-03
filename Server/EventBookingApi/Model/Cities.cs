using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;

public class Cities
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string CityName { get; set; } = null!;

    [Required]
    public string StateName { get; set; } = null!;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}