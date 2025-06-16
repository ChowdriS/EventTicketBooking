using System;
using EventBookingApi.Context;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class PaymentRepository : Repository<Guid, Payment>
{
    public PaymentRepository(EventContext context) : base(context) { }

    public override async Task<Payment> GetById(Guid id)
    {
        return await _eventContext.Payments.SingleOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Payment>> GetAll()
    {
        return await _eventContext.Payments.ToListAsync();
    }
}
