using System;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;

public interface ITicketTypeService
{
    Task<IEnumerable<TicketTypeResponseDTO>> GetAllTicketTypesForEvent(Guid reqId, Guid eventId);
    Task<TicketTypeResponseDTO> GetTicketTypeById(Guid reqId, Guid id);
    Task<TicketTypeResponseDTO> AddTicketType(Guid reqId, TicketTypeAddRequestDTO dto);
    Task<TicketTypeResponseDTO> UpdateTicketType(Guid reqId, Guid id, TicketTypeUpdateRequestDTO dto);
    Task<TicketTypeResponseDTO> DeleteTicketType(Guid reqId, Guid id);
}

