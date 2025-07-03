using System;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Service;

public class TicketTypeService : ITicketTypeService
{
    private readonly IRepository<Guid, TicketType> _ticketTypeRepository;
    private readonly IRepository<Guid, Event> _eventRepository;
    private readonly ObjectMapper _mapper;

    public TicketTypeService(IRepository<Guid, TicketType> ticketTypeRepository,
                             IRepository<Guid, Event> eventRepository,
                             ObjectMapper mapper)
    {
        _ticketTypeRepository = ticketTypeRepository;
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TicketTypeResponseDTO>> GetAllTicketTypesForEvent(Guid reqId ,Guid eventId)
    {
        var ev = await _eventRepository.GetById(eventId);
        var ticketTypes = await _ticketTypeRepository.GetAll();
        var filtered = ticketTypes.Where(t => t.EventId == eventId);
        return filtered.Select(_mapper.TicketTypeResponseDTOMapper);
    }

    public async Task<TicketTypeResponseDTO> GetTicketTypeById(Guid reqId,Guid id)
    {
        var ticketType = await _ticketTypeRepository.GetById(id);
        if(ticketType.IsDeleted)    throw new Exception("The TicketType is Already Deleted!");
        return _mapper.TicketTypeResponseDTOMapper(ticketType);
    }

    public async Task<TicketTypeResponseDTO> AddTicketType(Guid reqId, TicketTypeAddRequestDTO dto)
    {
        var ev = await _eventRepository.GetById(dto.EventId);
        if (ev.IsDeleted)
            throw new Exception("Associated event not found");
        if(reqId != ev.ManagerId)
            throw new Exception("Unauthorised Access");
        var ticketType = new TicketType
        {
            EventId = dto.EventId,
            TypeName = dto.TypeName,
            Price = dto.Price,
            TotalQuantity = dto.TotalQuantity,
            Description = dto.Description
        };

        ticketType = await _ticketTypeRepository.Add(ticketType);
        return _mapper.TicketTypeResponseDTOMapper(ticketType);
    }

    public async Task<TicketTypeResponseDTO> UpdateTicketType(Guid reqId, Guid id, TicketTypeUpdateRequestDTO dto)
    {
        var ticketType = await _ticketTypeRepository.GetById(id);
        var eventObj = await _eventRepository.GetById(ticketType.EventId);
        if (eventObj.ManagerId != reqId)    throw new Exception("UnAuthorised Access");

            if (dto.TotalQuantity.HasValue && dto.TotalQuantity < ticketType.BookedQuantity)
                throw new Exception("Total quantity cannot be less than booked quantity");

        ticketType.Price = dto.Price ?? ticketType.Price;
        ticketType.TotalQuantity = dto.TotalQuantity ?? ticketType.TotalQuantity;
        ticketType.Description = dto.Description ?? ticketType.Description;
        ticketType.UpdatedAt = DateTime.UtcNow;

        ticketType = await _ticketTypeRepository.Update(id, ticketType);
        return _mapper.TicketTypeResponseDTOMapper(ticketType);
    }

    public async Task<TicketTypeResponseDTO> DeleteTicketType(Guid reqId,Guid id)
    {
        var ticketType = await _ticketTypeRepository.GetById(id);
        var eventObj = await _eventRepository.GetById(ticketType.EventId);
        if (eventObj.ManagerId != reqId)    throw new Exception("UnAuthorised Access");
        ticketType.IsDeleted = true;
        ticketType = await _ticketTypeRepository.Update(id,ticketType);
        return _mapper.TicketTypeResponseDTOMapper(ticketType);
    }
}
