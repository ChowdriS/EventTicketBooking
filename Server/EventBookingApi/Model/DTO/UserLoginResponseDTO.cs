using System;


namespace EventBookingApi.Model.DTO;

public class UserLoginResponseDTO
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}
