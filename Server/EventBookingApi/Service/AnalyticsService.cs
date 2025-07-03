using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Service;

public class AnalyticsService : IAnalyticsService
{
    private readonly IRepository<Guid, Event> _eventRepository;
    private readonly IRepository<Guid, Ticket> _ticketRepository;
    private readonly IRepository<Guid, User> _userRepository;

    public AnalyticsService(
        IRepository<Guid, Event> eventRepository,
        IRepository<Guid, Ticket> ticketRepository,
        IRepository<Guid, User> userRepository)
    {
        _eventRepository = eventRepository;
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }
    // public async Task<Dictionary<string, decimal>> GetEventsTotalEarnings()
    // {
    //     var allTickets = await _ticketRepository.GetAll();
    //     var validTickets = allTickets.Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Used);
    //     var events = await _eventRepository.GetAll();
    //     var earnings = events.ToDictionary(
    //         e => e.Title ?? "",
    //         e => validTickets.Where(t => t.EventId == e.Id).Sum(t => t.TotalPrice)
    //     );

    //     return earnings;
    // }
    public async Task<Dictionary<string, decimal>> GetTotalEarnings(Guid managerId)
    {
        var allTickets = await _ticketRepository.GetAll();
        var validTickets = allTickets.Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Used);
        var events = await _eventRepository.GetAll();
        var managedEvents = events.Where(e => e.ManagerId == managerId).ToList();

        var earnings = managedEvents.ToDictionary(
            e => e.Title ?? "",
            e => validTickets.Where(t => t.EventId == e.Id).Sum(t => t.TotalPrice)
        );

        return earnings;
    }


    // public async Task<decimal> GetTotalEarnings(Guid managerId)
    // {
    //     var allTickets = await _ticketRepository.GetAll();
    //     var validTickets = allTickets.Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Used);
    //     var events = await _eventRepository.GetAll();
    //     var managedEventIds = events.Where(e => e.ManagerId == managerId).Select(e => e.Id);
    //     validTickets = validTickets.Where(t => managedEventIds.Contains(t.EventId));
    //     return validTickets.Sum(t => t.TotalPrice);
    // }

    public async Task<List<BookingTrendDTO>> GetBookingTrends()
    {
        var tickets = await _ticketRepository.GetAll();
        var grouped = tickets
            .Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Used)
            .GroupBy(t => t.BookedAt.Date)
            .Select(g => new BookingTrendDTO
            {
                Date = g.Key,
                TotalBookings = g.Count()
            })
            .OrderBy(b => b.Date)
            .ToList();

        return grouped;
    }

    public async Task<List<TopEventDTO>> GetTopEvents()
    {
        var tickets = await _ticketRepository.GetAll();
        var events = await _eventRepository.GetAll();

        var topEvents = tickets
            .Where(t => t.Status == TicketStatus.Booked || t.Status == TicketStatus.Used)
            .GroupBy(t => t.EventId)
            .Select(g => new
            {
                EventId = g.Key,
                TotalBookings = g.Sum(x => x.BookedQuantity)
            })
            .OrderByDescending(g => g.TotalBookings)
            .Take(10)
            .ToList();

        var result = topEvents
            .Join(events, t => t.EventId, e => e.Id, (t, e) => new TopEventDTO
            {
                EventId = e.Id,
                Title = e.Title ?? "",
                TotalBookings = t.TotalBookings
            })
            .ToList();

        return result;
    }
}
