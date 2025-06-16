using System;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Interface;

public interface ITicketService
{
    public Task<TicketResponseDTO> BookTicket(TicketBookRequestDTO dto, Guid userId);

    public Task<IEnumerable<TicketResponseDTO>> GetMyTickets(Guid userId, int pageNumber, int pageSize);

    public Task<TicketResponseDTO> GetTicketById(Guid ticketId, Guid userId);

    public Task<TicketResponseDTO> CancelTicket(Guid ticketId, Guid userId);

    public Task<byte[]> ExportTicketAsPdf(Guid ticketId, Guid userId);

    public Task<IEnumerable<TicketResponseDTO>> GetTicketsByEventId(Guid eventId, Guid requesterId, int pageNumber, int pageSize);
}
