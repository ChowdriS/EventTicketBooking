using System;
using System.ComponentModel.DataAnnotations;


namespace EventBookingApi.Model.DTO;

public class UserLoginRequestDTO
    {
        [Required(ErrorMessage = "Email is manditory"),EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is manditory")]
        public string Password { get; set; } = string.Empty;
    }
