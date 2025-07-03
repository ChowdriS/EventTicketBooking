using System;
using EventBookingApi.Context;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class CityRepository : Repository<Guid, Cities>
{
    public CityRepository(EventContext context) : base(context) { }

    public override async Task<Cities> GetById(Guid id)
    {
        var ev = await _eventContext.Cities
            .SingleOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            throw new KeyNotFoundException($"Event with id {id} not found.");

        return ev;
    }

    public override async Task<IEnumerable<Cities>> GetAll()
    {
        return await _eventContext.Cities.ToListAsync();
    }
}