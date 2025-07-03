using System;
using EventBookingApi.Context;
using EventBookingApi.Interface;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class EventImageRepository : Repository<Guid,EventImage>
{
    public EventImageRepository(EventContext context) : base(context) { }

    public override async Task<EventImage> GetById(Guid key)
    {
        var file = await _eventContext.EventImages.Include(e => e.Event).SingleOrDefaultAsync(i => i.Id == key);
        if (file == null)
        {
            throw new Exception("Id not Found");
        }
        return file;
    }

    public override async Task<IEnumerable<EventImage>> GetAll()
    {
        return await _eventContext.EventImages.Include(e=> e.Event).ToListAsync();
        // return files;
    }
}