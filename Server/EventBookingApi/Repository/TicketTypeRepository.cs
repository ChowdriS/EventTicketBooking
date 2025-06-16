using System;
using EventBookingApi.Context;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class TicketTypeRepository : Repository<Guid, TicketType>
{
    public TicketTypeRepository(EventContext context) : base(context) { }

    public override async Task<TicketType> GetById(Guid id)
    {
        return await _eventContext.TicketTypes.SingleOrDefaultAsync(tt => tt.Id == id);
    }

    public override async Task<IEnumerable<TicketType>> GetAll()
    {
        return await _eventContext.TicketTypes.ToListAsync();
    }
}
