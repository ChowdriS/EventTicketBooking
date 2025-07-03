using System;
using System.Security.Claims;
using System.Text.Json;
using EventBookingApi.Context;
using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventBookingApi.Misc;

public class OtherFunctionalities : IOtherFunctionalities
{
    private readonly EventContext _eventContext;
    private readonly ObjectMapper _mapper;
    private readonly ITicketService _ticketService;

    public OtherFunctionalities(EventContext eventContext, ObjectMapper mapper, ITicketService ticketService)
    {
        _eventContext = eventContext;
        _mapper = mapper;
        _ticketService = ticketService;
    }
    public Guid GetLoggedInUserId(ClaimsPrincipal User)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            // System.Console.WriteLine(userId);
            return userId;
        }
        else
        {
            throw new Exception("Invalid or missing user ID in token.");
        }
    }
    public async Task<PaginatedResultDTO<TicketResponseDTO>> GetPaginatedTicketsByEventId(Guid eventId, Guid requesterId, int pageNumber, int pageSize)
    {
        var evt = await _eventContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        var user = await _eventContext.Users.FirstOrDefaultAsync(e => e.Id == requesterId);

        if (user!.Role == UserRole.Manager && evt!.ManagerId != requesterId)
            throw new UnauthorizedAccessException("Access denied");

        var query = _eventContext.Tickets
            .Where(t => t.EventId == eventId)
            .OrderByDescending(t => t.BookedAt);

        var totalItems = await query.CountAsync();

        var paginatedTickets = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var responses = new List<TicketResponseDTO>();

        foreach (var ticket in paginatedTickets)
        {
            var response = await _ticketService.GetTicketById(ticket.Id, requesterId);
            responses.Add(response);
        }

        return new PaginatedResultDTO<TicketResponseDTO>
        {
            Items = responses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedResultDTO<TicketResponseDTO>> GetPaginatedMyTickets(Guid userId, int pageNumber, int pageSize)
    {
        var query = _eventContext.Tickets
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.BookedAt);

        var totalItems = await query.CountAsync();

        var paginatedTickets = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var responses = new List<TicketResponseDTO>();

        foreach (var ticket in paginatedTickets)
        {
            var response = await _ticketService.GetTicketById(ticket.Id, userId);
            responses.Add(response);
        }

        return new PaginatedResultDTO<TicketResponseDTO>
        {
            Items = responses,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEvents(int pageNumber, int pageSize)
    {
        var query = _eventContext.Events
                .Where(e => !e.IsDeleted)
                .Include(e => e.TicketTypes)
                .Include(e => e.Tickets)
                .Include(e => e.BookedSeats)
                .Include(e => e.Images)
                .Include(e => e.City)
                .OrderByDescending(e => e.EventDate);


        var totalItems = await query.CountAsync();

        var events = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var newevents = _mapper.ManyEvenetResponseDTOMapper(events);

        return new PaginatedResultDTO<EventResponseDTO>
        {
            Items = newevents,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEventsByManager(Guid managerId, int pageNumber, int pageSize)
    {
        var query = _eventContext.Events
            .Where(e => e.ManagerId == managerId)
            .Include(e => e.TicketTypes)
            .Include(e => e.Tickets)
            .Include(e => e.BookedSeats)
            .Include(e => e.Images)
            .Include(e => e.City)
            .OrderByDescending(e => e.EventDate);

        var totalItems = await query.CountAsync();

        var events = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var newevents = _mapper.ManyEvenetResponseDTOMapper(events);

        return new PaginatedResultDTO<EventResponseDTO>
        {
            Items = newevents,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
    public async Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEventsByFilter(EventCategory? category, Guid? cityId,EventType? type,string? searchElement, DateTime? date, int pageNumber, int pageSize)
    {
        // System.Console.WriteLine(EventType.Seatable == type);
        // System.Console.WriteLine(0 == type);
        var query = _eventContext.Events.Where(e =>
                (string.IsNullOrEmpty(searchElement) || e.Description!.ToLower().Contains(searchElement.ToLower()) ||
                 e.Title!.ToLower().Contains(searchElement.ToLower())) &&
                (!date.HasValue || e.EventDate.Date == date.Value.Date) &&
                (type ==null || type == e.EventType) &&
                (category == null || category == e.Category) &&
                (cityId == null || e.CityId == cityId))
                .Include(e => e.TicketTypes)
                .Include(e => e.Tickets)
                .Include(e => e.BookedSeats)
                .Include(e => e.Images)
                .Include(e => e.City)
                .OrderByDescending(e => e.EventDate);

        var events = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalItems = await query.CountAsync();
        var newevents = _mapper.ManyEvenetResponseDTOMapper(events);

        return new PaginatedResultDTO<EventResponseDTO>
        {
            Items = newevents,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}
