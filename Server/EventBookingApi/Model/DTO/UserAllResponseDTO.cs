using System;

namespace EventBookingApi.Model.DTO;

public class UserAllResponseDTO
{
    public Guid? Id { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public bool IsDeleted { get; set; }
}
