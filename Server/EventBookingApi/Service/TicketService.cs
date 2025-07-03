using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using EventBookingApi.Misc;


namespace EventBookingApi.Service;

public class TicketService : ITicketService
{
    private readonly IRepository<Guid, Ticket> _ticketRepository;
    private readonly IRepository<Guid, Event> _eventRepository;
    private readonly IRepository<Guid, TicketType> _ticketTypeRepository;
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, BookedSeat> _bookedSeatRepository;
    private readonly IRepository<Guid, Payment> _paymentRepository;
    private readonly INotificationService _notificationService;
    private readonly ObjectMapper _mapper;

    public TicketService(
        IRepository<Guid, Ticket> ticketRepository,
        IRepository<Guid, Event> eventRepository,
        IRepository<Guid, TicketType> ticketTypeRepository,
        IRepository<Guid, User> userRepository,
        IRepository<Guid, BookedSeat> bookedSeatRepository,
        IRepository<Guid, Payment> paymentRepository,
        INotificationService notificationService,
        ObjectMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _eventRepository = eventRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _userRepository = userRepository;
        _bookedSeatRepository = bookedSeatRepository;
        _paymentRepository = paymentRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<TicketResponseDTO> BookTicket(TicketBookRequestDTO dto, Guid userId)
    {
        var eventObj = await _eventRepository.GetById(dto.EventId);
        if (eventObj == null || eventObj.IsDeleted || eventObj.EventStatus != EventStatus.Active)
            throw new Exception("Event not available");

        var ticketType = await _ticketTypeRepository.GetById(dto.TicketTypeId);
        if (ticketType.IsDeleted == true)
            throw new Exception("Ticket type not found");
        if (ticketType.TotalQuantity - ticketType.BookedQuantity <= 0)
        {
            await _notificationService.NotifyAdmins(
            $"Event '{eventObj.Title}' is sold out",
            "EventFull"
            );
            await _notificationService.NotifyEventManagers(
                eventObj.Id,
                $"Your event '{eventObj.Title}' is sold out",
                "EventFull"
            );
            throw new Exception("No tickets available in that ticketType in the requested Event");
        }
        if (eventObj.EventType == EventType.Seatable)
        {
            return await BookSeatableTicket(dto, userId, eventObj, ticketType);
        }
        else
        {
            return await BookNonSeatableTicket(dto, userId, eventObj, ticketType);
        }
    }
    private async Task<Payment> CreatePayment(PaymentRequestDTO? paymentDto, decimal amount, Guid ticketId)
    {
        var payment = new Payment
        {
            PaymentType = paymentDto!.PaymentType,
            Amount = amount,
            PaymentStatus = PaymentStatusEnum.Paid,
            TransactionId = paymentDto.TransactionId,
            TicketId = ticketId
        };

        return await _paymentRepository.Add(payment);
    }
    private async Task<TicketResponseDTO> BookSeatableTicket(
        TicketBookRequestDTO dto, Guid userId, Event eventObj, TicketType ticketType)
    {
        if (dto.SeatNumbers == null || dto.SeatNumbers.Count != dto.Quantity)
            throw new Exception("For seatable events, you must provide exactly as many seat numbers as the quantity");

        if (ticketType.TotalQuantity - ticketType.BookedQuantity < dto.Quantity)
            throw new Exception("Not enough tickets available");
        var existingSeats = (await _bookedSeatRepository.GetAll())
            .Where(bs => bs.EventId == eventObj.Id &&
                         dto.SeatNumbers.Contains(bs.SeatNumber) &&
                         bs.BookedSeatStatus == BookedSeatStatus.Booked)
            .ToList();

        if (existingSeats.Any())
        {
            var takenSeats = string.Join(", ", existingSeats.Select(s => s.SeatNumber));
            throw new Exception($"Seats {takenSeats} are already booked");
        }

        ticketType.BookedQuantity += dto.Quantity;
        await _ticketTypeRepository.Update(ticketType.Id, ticketType);

        var ticket = new Ticket
        {
            UserId = userId,
            EventId = eventObj.Id,
            TicketTypeId = ticketType.Id,
            BookedQuantity = dto.Quantity,
            TotalPrice = dto.Quantity * ticketType.Price,
            Status = TicketStatus.Booked,
            PaymentId = null
        };

        var createdTicket = await _ticketRepository.Add(ticket);
        var payment = await CreatePayment(dto.Payment, createdTicket.TotalPrice, createdTicket.Id);
        createdTicket.PaymentId = payment.Id;
        await _ticketRepository.Update(createdTicket.Id, createdTicket);

        foreach (var seatNumber in dto.SeatNumbers)
        {
            var bookedSeat = new BookedSeat
            {
                EventId = eventObj.Id,
                TicketId = createdTicket.Id,
                SeatNumber = seatNumber,
                BookedSeatStatus = BookedSeatStatus.Booked
            };
            await _bookedSeatRepository.Add(bookedSeat);
        }

        await _notificationService.NotifyUser(
            userId,
            $"Ticket booked for {eventObj.Title}",
            "TicketBooked"
        );
        return _mapper.TicketResponseDTOMapper(ticket, eventObj, ticketType, payment);
    }

    private async Task<TicketResponseDTO> BookNonSeatableTicket(
        TicketBookRequestDTO dto, Guid userId, Event eventObj, TicketType ticketType)
    {
        if (dto.SeatNumbers != null && dto.SeatNumbers.Any())
            throw new Exception("Seat numbers should not be provided for non-seatable events");

        if (ticketType.TotalQuantity - ticketType.BookedQuantity < dto.Quantity)
            throw new Exception("Not enough tickets available");

        ticketType.BookedQuantity += dto.Quantity;
        await _ticketTypeRepository.Update(ticketType.Id, ticketType);

        var ticket = new Ticket
        {
            UserId = userId,
            EventId = eventObj.Id,
            TicketTypeId = ticketType.Id,
            BookedQuantity = dto.Quantity,
            TotalPrice = dto.Quantity * ticketType.Price,
            Status = TicketStatus.Booked,
            PaymentId = null
        };

        var createdTicket = await _ticketRepository.Add(ticket);

        var payment = await CreatePayment(dto.Payment, createdTicket.TotalPrice, createdTicket.Id);

        createdTicket.PaymentId = payment.Id;
        await _ticketRepository.Update(createdTicket.Id, createdTicket);
        
        await _notificationService.NotifyUser(
            userId,
            $"Ticket booked for {eventObj.Title}",
            "TicketBooked"
        );
        return _mapper.TicketResponseDTOMapper(ticket, eventObj, ticketType, payment);
    }

    public async Task<TicketResponseDTO> CancelTicket(Guid ticketId, Guid userId)
    {
        var ticket = await _ticketRepository.GetById(ticketId);
        if (ticket == null || ticket.UserId != userId)
            throw new UnauthorizedAccessException("Cannot cancel this ticket");

        if (ticket.Status != TicketStatus.Booked)
            throw new Exception("Ticket is not in a cancellable state");

        var eventObj = await _eventRepository.GetById(ticket.EventId);
        var ticketType = await _ticketTypeRepository.GetById(ticket.TicketTypeId);
        var payment = ticket.PaymentId.HasValue
            ? await _paymentRepository.GetById(ticket.PaymentId.Value)
            : null;
        List<BookedSeat>? bookedSeats = null;
        if (eventObj.EventType == EventType.Seatable)
        {
            bookedSeats = (await _bookedSeatRepository.GetAll())
                .Where(bs => bs.TicketId == ticketId)
                .ToList();

            foreach (var seat in bookedSeats)
            {
                seat.BookedSeatStatus = BookedSeatStatus.Cancelled;
                await _bookedSeatRepository.Update(seat.Id, seat);
            }
        }

        ticketType.BookedQuantity -= ticket.BookedQuantity;
        await _ticketTypeRepository.Update(ticketType.Id, ticketType);
        if (payment != null)
        {
            payment.PaymentStatus = PaymentStatusEnum.Refund;
            await _paymentRepository.Update(payment.Id, payment);
        }
        ticket.Status = TicketStatus.Cancelled;
        await _ticketRepository.Update(ticketId, ticket);
        await _notificationService.NotifyUser(
            userId,
            $"Ticket canceled for {eventObj.Title}",
            "TicketCancelled"
        );
        return _mapper.TicketResponseDTOMapper(ticket, eventObj,ticketType, payment);
    }

    public async Task<IEnumerable<TicketResponseDTO>> GetMyTickets(Guid userId, int pageNumber, int pageSize)
    {
        var tickets = (await _ticketRepository.GetAll())
            .Where(t => t.UserId == userId)
            .ToList();
        var responses = new List<TicketResponseDTO>();
        foreach (var ticket in tickets)
        {
            var response = await GetTicketById(ticket.Id, userId);
            responses.Add(response);
        }
        // var response = await _otherFunctionalities.GetPaginatedMyTickets(userId,pageNumber,pageSize);
        return responses;
    }
    public async Task<TicketResponseDTO> GetTicketById(Guid ticketId, Guid userId)
    {
        var ticket = await _ticketRepository.GetById(ticketId);
        var user = await _userRepository.GetById(userId);
        if (ticket == null || (ticket.UserId != userId && ticket?.Event?.ManagerId != userId && user.Role != UserRole.Admin))
            throw new UnauthorizedAccessException("Access denied");
        // if (ticket.Status == TicketStatus.Cancelled) throw new Exception("The Ticket is already Cancelled!");
        var eventObj = await _eventRepository.GetById(ticket!.EventId);
        var ticketType = await _ticketTypeRepository.GetById(ticket.TicketTypeId);
        Payment? payment = ticket.PaymentId.HasValue
        ? await _paymentRepository.GetById(ticket.PaymentId.Value)
        : null;
        var response = _mapper.TicketResponseDTOMapper(ticket, eventObj,ticketType, payment);

        if (eventObj?.EventType == EventType.Seatable)
        {
            var bookedSeats = (await _bookedSeatRepository.GetAll())
                .Where(bs => bs.TicketId == ticket.Id &&
                             bs.BookedSeatStatus == BookedSeatStatus.Booked)
                .Select(bs => bs.SeatNumber)
                .ToList();

            response.SeatNumbers = bookedSeats;
        }

        return response;
    }
    public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByEventId(Guid eventId, Guid requesterId,int pageNumber, int pageSize)
    {
        var evt = await _eventRepository.GetById(eventId);
        var user = await _userRepository.GetById(requesterId);
        if (user == null) throw new Exception("User not found");
        if (evt == null) throw new Exception("Event not found");
        if (user.Role != UserRole.Admin && evt.ManagerId != requesterId)
            throw new UnauthorizedAccessException("Access denied");
        var tickets = (await _ticketRepository.GetAll())
            .Where(t => t.EventId == eventId)
            .ToList();
        var responses = new List<TicketResponseDTO>();
        foreach (var ticket in tickets)
        {
            var response = await GetTicketById(ticket.Id, requesterId);
            responses.Add(response);
        }
        // var responses = await _otherFunctionalities.GetPaginatedTicketsByEventId(eventId, requesterId, pageNumber, pageSize);
        return responses;
    }
    public async Task<byte[]> ExportTicketAsPdf(Guid ticketId, Guid userId)
    {
        var ticket = await _ticketRepository.GetById(ticketId);
        if (ticket == null || ticket.UserId != userId)
            throw new UnauthorizedAccessException("Unauthorized");
        if (ticket.Status == TicketStatus.Cancelled)
        {
            throw new Exception("The Ticket is Already Cancelled!");
        }
        var eventObj = await _eventRepository.GetById(ticket.EventId);
        var ticketType = await _ticketTypeRepository.GetById(ticket.TicketTypeId);
        var payment = ticket.PaymentId.HasValue
            ? await _paymentRepository.GetById(ticket.PaymentId.Value)
            : null;

        List<int>? bookedSeatNumbers = null;
        if (eventObj?.EventType == EventType.Seatable)
        {
            bookedSeatNumbers = (await _bookedSeatRepository.GetAll())
                .Where(bs => bs.TicketId == ticketId &&
                            bs.BookedSeatStatus == BookedSeatStatus.Booked)
                .Select(bs => bs.SeatNumber)
                .ToList();
        }
        await _notificationService.NotifyUser(
            userId,
            $"Ticket Generated for {eventObj?.Title}",
            "Success"
        );
        using (var ms = new System.IO.MemoryStream())
        {
            var document = new PdfDocument();  
            var page = document.AddPage();    
            var gfx = XGraphics.FromPdfPage(page); 
            var font = new XFont("Arial", 12);

            double xPos = 40;
            double yPos = 50;

            gfx.DrawString("TICKET RECEIPT", new XFont("Arial", 18, XFontStyle.Bold), XBrushes.Black, new XRect(xPos, yPos, page.Width, page.Height), XStringFormats.TopCenter);
            yPos += 40;
            gfx.DrawString("------------------------------------------------------------------------------------------------------------------------------------", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            gfx.DrawString($"Ticket Id : {ticket.Id}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;
            gfx.DrawString($"Event: {eventObj?.Title}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            gfx.DrawString($"Ticket Type: {ticketType?.TypeName}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            gfx.DrawString($"Quantity: {ticket.BookedQuantity}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            gfx.DrawString($"Total Price: Rs.{ticket.TotalPrice}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            if (payment != null)
            {
                gfx.DrawString($"Payment Method: {payment.PaymentType}", font, XBrushes.Black, xPos, yPos);
                yPos += 20;

                gfx.DrawString($"Transaction ID: {payment.TransactionId}", font, XBrushes.Black, xPos, yPos);
                yPos += 20;

                gfx.DrawString($"Payment Status: {payment.PaymentStatus}", font, XBrushes.Black, xPos, yPos);
                yPos += 20;
            }

            gfx.DrawString($"Booked At: {ticket.BookedAt:g}", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            if (bookedSeatNumbers?.Any() == true)
            {
                gfx.DrawString($"Seat Numbers: {string.Join(", ", bookedSeatNumbers)}", font, XBrushes.Black, xPos, yPos);
                yPos += 20;
            }

            gfx.DrawString("------------------------------------------------------------------------------------------------------------------------------------", font, XBrushes.Black, xPos, yPos);
            yPos += 20;

            gfx.DrawString("Thank you for your booking!", new XFont("Arial", 12, XFontStyle.Italic), XBrushes.Black, new XRect(xPos, yPos, page.Width, page.Height), XStringFormats.TopCenter);

            document.Save(ms);
            return ms.ToArray();
        }
    }
}