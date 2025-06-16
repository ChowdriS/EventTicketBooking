using System;
using EventBookingApi.Model;

namespace EventBookingApi.Model.DTO
{
    public class PaymentDetailDTO : PaymentResponseDTO
    {
        public Guid EventId { get; set; }
        public string? EventTitle { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime BookedAt { get; set; }
        public TicketStatus TicketStatus { get; set; }
    }
}