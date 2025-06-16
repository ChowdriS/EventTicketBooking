using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? RefreshToken { get; set; } = null;
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<Event>? ManagedEvents { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
}