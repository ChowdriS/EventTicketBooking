using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model.DTO;

public class PaymentRequestDTO
{
    [Required]
    public PaymentTypeEnum PaymentType { get; set; }
    
    [Required]
    public Guid TransactionId { get; set; }
}