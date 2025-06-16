using System;
using EventBookingApi.Context;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class TicketRepository : Repository<Guid, Ticket>
{
    public TicketRepository(EventContext context) : base(context) { }

    public override async Task<Ticket> GetById(Guid id)
    {
        return await _eventContext.Tickets.SingleOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IEnumerable<Ticket>> GetAll()
    {
        return await _eventContext.Tickets.ToListAsync();
    }
}
