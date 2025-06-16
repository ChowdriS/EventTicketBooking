using System;
using EventBookingApi.Context;
using EventBookingApi.Interface;
using EventBookingApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Repository;

public class BookedSeatRepository : Repository<Guid, BookedSeat>
{
    public BookedSeatRepository(EventContext context) : base(context) { }

    public override async Task<BookedSeat> GetById(Guid id)
    {
        return await _eventContext.BookedSeats.SingleOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<BookedSeat>> GetAll()
    {
        return await _eventContext.BookedSeats.ToListAsync();
    }
}