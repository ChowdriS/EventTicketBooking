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
        var ev = await _eventContext.Events
            .Include(e => e.TicketTypes)
            .Include(e => e.Tickets)
            .Include(e => e.BookedSeats)
            .Include(e => e.Images)
            .Include(e => e.City)
            .SingleOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            throw new KeyNotFoundException($"Event with id {id} not found.");

        return ev;
    }

    public override async Task<IEnumerable<Event>> GetAll()
    {
        return await _eventContext.Events
            .Include(e => e.TicketTypes)
            .Include(e => e.Tickets)
            .Include(e => e.Images)
            .Include(e => e.City)
            .Include(e => e.BookedSeats).ToListAsync();
    }
}
