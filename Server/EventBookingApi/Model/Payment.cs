using System;
using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.Model;

public class Payment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public decimal Amount { get; set; }
    public PaymentTypeEnum PaymentType { get; set; } = PaymentTypeEnum.UPI;
    public PaymentStatusEnum PaymentStatus { get; set; }
    public Guid TransactionId { get; set; } = new Guid();
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}