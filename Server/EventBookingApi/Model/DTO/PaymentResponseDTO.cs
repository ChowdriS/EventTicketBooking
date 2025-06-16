using System;

namespace EventBookingApi.Model.DTO;

public class PaymentResponseDTO
{
    public Guid Id { get; set; }
    public PaymentTypeEnum PaymentType { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public Guid? TransactionId { get; set; }
}