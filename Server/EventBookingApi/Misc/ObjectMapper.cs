using System;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;

namespace EventBookingApi.Misc;

public class ObjectMapper
{
    public TicketResponseDTO TicketResponseDTOMapper(Ticket ticket, Event eventObj, TicketType ticketType, Payment? payment) => new()
    {
        Id = ticket.Id,
        UserId = ticket.UserId,
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
    public UserAllResponseDTO UserALLResponseDTOMapper(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Username = user.Username,
        Role = user.Role.ToString(),
        IsDeleted = user.IsDeleted
    };

    public ICollection<EventResponseTicketTypeDTO> TypeMapper(ICollection<TicketType>? types)
    {
        ICollection<EventResponseTicketTypeDTO> response = [];
        if (types == null)
        {
            return response;
        }
        foreach (var type in types!)
        {
            response.Add(EventResponseTicketTypeDTOMapper(type));
        }
        return response;
    }
    public ICollection<EventResponseBookedSeatDTO> SeatsMapper(ICollection<BookedSeat>? seats)
    {
        ICollection<EventResponseBookedSeatDTO> response = [];
        if (seats == null)
        {
            return response;
        }
        foreach (var seat in seats!)
        {
            response.Add(EventResponseBookedSeatsDTOMapper(seat));
        }
        return response;
    }
    public ICollection<Guid> ImageMapper(ICollection<EventImage>? images)
    {
        ICollection<Guid> response = [];
        if (images != null)
        {
            foreach (var item in images)
            {
                response.Add(item.Id);
            }
        }
        return response;
    }
    public EventResponseDTO EvenetResponseDTOMapper(Event ev) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        EventDate = ev.EventDate,
        EventStatus = ev.EventStatus.ToString(),
        EventType = ev.EventType.ToString(),
        TicketTypes = TypeMapper(ev.TicketTypes),
        BookedSeats = SeatsMapper(ev.BookedSeats),
        Images = ImageMapper(ev.Images),
        Location = ev.City.CityName + ", " + ev.City.StateName,
        Category = ev.Category.ToString()
    };

    public EventResponseTicketTypeDTO EventResponseTicketTypeDTOMapper(TicketType type) => new()
    {
        Id = type.Id,
        TypeName = type.TypeName,
        Price = type.Price,
        TotalQuantity = type.TotalQuantity,
        BookedQuantity = type.BookedQuantity,
        Description = type.Description,
        IsDeleted = type.IsDeleted
    };
    public EventResponseBookedSeatDTO EventResponseBookedSeatsDTOMapper(BookedSeat seat) => new()
    {
        SeatNumber = seat.SeatNumber,
        BookedSeatStatus = seat.BookedSeatStatus
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
