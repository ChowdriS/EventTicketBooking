using System;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Misc;

public class ObjectMapper
{
    public TicketResponseDTO TicketResponseDTOMapper(Ticket ticket, Event eventObj, TicketType ticketType, Payment? payment) => new()
    {
        Id = ticket.Id,
        EventTitle = eventObj?.Title ?? "",
        TicketType = ticketType?.TypeName.ToString() ?? "",
        PricePerTicket = ticketType?.Price ?? 0,
        Quantity = ticket.BookedQuantity,
        BookedAt = ticket.BookedAt,
        Payment = payment != null ? new PaymentResponseDTO
        {
            Id = payment.Id,
            PaymentType = payment.PaymentType,
            Amount = payment.Amount,
            Status = payment.PaymentStatus,
            TransactionId = payment.TransactionId
        } : null
    };
    public UserResponseDTO UserResponseDTOMapper(User user) => new()
    {
        Email = user.Email,
        Username = user.Username,
        Role = user.Role.ToString()
    };
    public EventResponseDTO EvenetResponseDTOMapper(Event ev) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        EventDate = ev.EventDate,
        EventStatus = ev.EventStatus.ToString(),
        EventType = ev.EventType.ToString()
    };

    public IEnumerable<EventResponseDTO> ManyEvenetResponseDTOMapper(IEnumerable<Event> ev)
    {
        var responses = new List<EventResponseDTO>();
        foreach (var ev2 in ev)
        {
            responses.Add(EvenetResponseDTOMapper(ev2));
        }
        return responses;
    }

    public PaymentDetailDTO PaymentDetailDTOMapper(Payment payment, Event eventObj, User user, Ticket ticket) => new PaymentDetailDTO
    {
        Id = payment.Id,
        PaymentType = payment.PaymentType,
        Amount = payment.Amount,
        Status = payment.PaymentStatus,
        TransactionId = payment.TransactionId,
        EventId = eventObj.Id,
        EventTitle = eventObj.Title ?? "",
        UserId = user.Id,
        UserName = user.Username ?? "",
        UserEmail = user.Email ?? "",
        BookedAt = ticket.BookedAt,
        TicketStatus = ticket.Status
    };
    
    public virtual TicketTypeResponseDTO TicketTypeResponseDTOMapper(TicketType ticketType)
    {
        return new TicketTypeResponseDTO
        {
            Id = ticketType.Id,
            EventId = ticketType.EventId,
            TypeName = ticketType.TypeName,
            Price = ticketType.Price,
            TotalQuantity = ticketType.TotalQuantity,
            BookedQuantity = ticketType.BookedQuantity,
            Description = ticketType.Description,
            CreatedAt = ticketType.CreatedAt,
            IsDeleted = ticketType.IsDeleted
        };
    }
}
