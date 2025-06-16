using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model.DTO;

public class ChangePasswordDTO
{
    [Required]
    public string? oldPassword { get; set; }
    [Required]
    public string? newPassword { get; set;}
}
