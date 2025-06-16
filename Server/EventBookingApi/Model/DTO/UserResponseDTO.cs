using System;

namespace EventBookingApi.Model.DTO;

public class UserResponseDTO
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
}
