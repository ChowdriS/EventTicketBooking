using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface
{
    public interface IPaymentService
    {
        public Task<PaymentDetailDTO> GetPaymentByTicketId(Guid ticketId, Guid requesterId);
        public Task<PaymentDetailDTO> GetPaymentById(Guid paymentId, Guid? requesterId);
        public Task<IEnumerable<PaymentDetailDTO>> GetPaymentsByUserId(Guid userId);
        public Task<IEnumerable<PaymentDetailDTO>> GetPaymentsByEventId(Guid eventId, Guid? managerId);
    }
}