using System;
using System.Security.Claims;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;

public interface IOtherFunctionalities
{
    public Task<PaginatedResultDTO<TicketResponseDTO>> GetPaginatedTicketsByEventId(Guid eventId, Guid requesterId, int pageNumber, int pageSize);
    public Task<PaginatedResultDTO<TicketResponseDTO>> GetPaginatedMyTickets(Guid userId, int pageNumber, int pageSize);
    public Guid GetLoggedInUserId(ClaimsPrincipal User);
    public Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEvents(int pageNumber, int pageSize);
    public Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEventsByManager(Guid managerId, int pageNumber, int pageSize);
    public Task<PaginatedResultDTO<EventResponseDTO>> GetPaginatedEventsByFilter(EventCategory? category, Guid? cityId,EventType? type, string? searchElement, DateTime? date, int pageNumber, int pageSize);
}
