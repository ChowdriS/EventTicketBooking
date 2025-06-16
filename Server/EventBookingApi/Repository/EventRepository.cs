using System;
using EventBookingApi.Context;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class EventRepository : Repository<Guid, Event>
{
    public EventRepository(EventContext context) : base(context) { }

    public override async Task<Event> GetById(Guid id)
    {
        return await _eventContext.Events.SingleOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<Event>> GetAll()
    {
        return await _eventContext.Events.ToListAsync();
    }
}
