using System;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Service;

public class EventService : IEventService
{
    private readonly IRepository<Guid, Event> _eventRepository;
    private readonly IRepository<Guid, TicketType> _ticketTypeRepository;
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IOtherFunctionalities _otherFunctionalities;
    private readonly ObjectMapper _mapper;

    public EventService(IRepository<Guid, Event> eventRepository,
                        IRepository<Guid, TicketType> ticketTypeRepository,
                        IRepository<Guid, User> userRepository,
                        IOtherFunctionalities otherFunctionalities,
                        ObjectMapper mapper)
    {
        _eventRepository = eventRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _userRepository = userRepository;
        _otherFunctionalities = otherFunctionalities;
        _mapper = mapper;
    }

    public async Task<PaginatedResultDTO<EventResponseDTO>> GetAllEvents(int pageNumber, int pageSize)
    {
        return await _otherFunctionalities.GetPaginatedEvents(pageNumber, pageSize);
    }

    public async Task<EventResponseDTO> GetEventById(Guid id)
    {
        var ev = await _eventRepository.GetById(id);
        if (ev == null || ev.IsDeleted)
            throw new Exception("Event not found");

        return _mapper.EvenetResponseDTOMapper(ev);
    }

    public async Task<PaginatedResultDTO<EventResponseDTO>> FilterEvents(string searchElement, DateTime? date, int pageNumber, int pageSize)
    {
        return await _otherFunctionalities.GetPaginatedEventsByFilter(searchElement, date, pageNumber, pageSize);
    }

    public async Task<PaginatedResultDTO<EventResponseDTO>> GetManagedEventsByUserId(Guid managerId, int pageNumber, int pageSize)
    {
        return await _otherFunctionalities.GetPaginatedEventsByManager(managerId, pageNumber, pageSize);
    }

    public async Task<EventResponseDTO> AddEvent(EventAddRequestDTO dto,Guid ManagerId)
    {
        var manager = await _userRepository.GetById(ManagerId);

        var newEvent = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            EventType = dto.EventType,
            EventDate = dto.EventDate,
            ManagerId = manager?.Id
        };

        newEvent = await _eventRepository.Add(newEvent);

        if (dto.TicketTypes != null && dto.TicketTypes.Any())
        {
            foreach (var ticketDto in dto.TicketTypes)
            {
                var ticketType = new TicketType
                {
                    EventId = newEvent.Id,
                    TypeName = ticketDto.TypeName,
                    Price = ticketDto.Price,
                    TotalQuantity = ticketDto.TotalQuantity,
                    Description = ticketDto.Description
                };
                await _ticketTypeRepository.Add(ticketType);
            }
        }

        return _mapper.EvenetResponseDTOMapper(newEvent);
    }

    public async Task<EventResponseDTO> UpdateEvent(Guid id, EventUpdateRequestDTO dto)
    {
        var existingEvent = await _eventRepository.GetById(id);
        existingEvent.Title = dto.Title ?? existingEvent.Title;
        existingEvent.Description = dto.Description ?? existingEvent.Description;
        existingEvent.EventDate = dto.EventDate ?? existingEvent.EventDate;
        existingEvent.EventType = dto.EventType ?? existingEvent.EventType; 
        existingEvent.UpdatedAt = DateTime.UtcNow;
        existingEvent.EventStatus = dto.EventStatus ?? existingEvent.EventStatus;
        var updatedEvent = await _eventRepository.Update(id, existingEvent);
        return _mapper.EvenetResponseDTOMapper(updatedEvent);
    }

    public async Task<EventResponseDTO> DeleteEvent(Guid id)
    {
        var existingEvent = await _eventRepository.GetById(id);
        existingEvent.IsDeleted = true;
        existingEvent.UpdatedAt = DateTime.UtcNow;
        existingEvent.EventStatus = EventStatus.Cancelled;
        await _eventRepository.Update(id, existingEvent);
        return _mapper.EvenetResponseDTOMapper(existingEvent);
    }
}
