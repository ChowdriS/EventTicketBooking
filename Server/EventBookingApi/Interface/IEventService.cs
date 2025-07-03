using System;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;

public interface IEventService
{
    // public Task<EventResponseDTO> UpdateEventImageUrl(Guid eventId, IFormFile imageFile);
    public Task<IEnumerable<Cities>> getAllCities();
    public Task<PaginatedResultDTO<EventResponseDTO>> GetAllEvents(int pageNumber, int pageSize);
    public Task<EventResponseDTO> GetEventById(Guid id);
    public Task<PaginatedResultDTO<EventResponseDTO>> FilterEvents(EventCategory? category, Guid? cityId,EventType? type, string searchElement, DateTime? date, int pageNumber, int pageSize);
    public Task<PaginatedResultDTO<EventResponseDTO>> GetManagedEventsByUserId(Guid managerId, int pageNumber, int pageSize);
    public Task<EventResponseDTO> AddEvent(EventAddRequestDTO dto,Guid ManagerId);
    public Task<EventResponseDTO> UpdateEvent(Guid id, EventUpdateRequestDTO dto);
    public Task<EventResponseDTO> DeleteEvent(Guid id);
}
