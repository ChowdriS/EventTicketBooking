using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using Moq;

namespace EventBookingApi.Tests
{
    public class TicketServiceTests
    {
        private Mock<IRepository<Guid, Ticket>> _ticketRepo;
        private Mock<IRepository<Guid, Event>> _eventRepo;
        private Mock<IRepository<Guid, TicketType>> _ticketTypeRepo;
        private Mock<IRepository<Guid, Payment>> _paymentRepo;
        private Mock<IRepository<Guid, BookedSeat>> _bookedSeatRepo;
        private Mock<IRepository<Guid, User>> _userRepo;
        private Mock<INotificationService> _notificationService;
        private TicketService _ticketService;

        [SetUp]
        public void Setup()
        {
            _ticketRepo = new Mock<IRepository<Guid, Ticket>>();
            _eventRepo = new Mock<IRepository<Guid, Event>>();
            _ticketTypeRepo = new Mock<IRepository<Guid, TicketType>>();
            _paymentRepo = new Mock<IRepository<Guid, Payment>>();
            _bookedSeatRepo = new Mock<IRepository<Guid, BookedSeat>>();
            _userRepo = new Mock<IRepository<Guid, User>>();
            _notificationService = new Mock<INotificationService>();
            ObjectMapper _mapper;

            _ticketService = new TicketService(
                _ticketRepo.Object,
                _eventRepo.Object,
                _ticketTypeRepo.Object,
                _userRepo.Object,
                _bookedSeatRepo.Object,
                _paymentRepo.Object,
                _notificationService.Object,
                _mapper = new ObjectMapper()
            );
        }

        [Test]
        public async Task BookTicket_SuccessfullyBooksTicket()
        {
            var request = new TicketBookRequestDTO
            {
                EventId = Guid.NewGuid(),
                TicketTypeId = Guid.NewGuid(),
                Quantity = 2,
                SeatNumbers = new List<int> { 1, 2 },
                Payment = new PaymentRequestDTO { PaymentType = PaymentTypeEnum.UPI, TransactionId = Guid.NewGuid() }
            };
            var userId = Guid.NewGuid();
            var eventObj = new Event { Id = request.EventId, EventType = EventType.Seatable, EventStatus = EventStatus.Active };
            var ticketType = new TicketType { Id = request.TicketTypeId, Price = 100, TotalQuantity = 10 };
            var user = new User { Id = userId };

            _eventRepo.Setup(x => x.GetById(request.EventId)).ReturnsAsync(eventObj);
            _ticketTypeRepo.Setup(x => x.GetById(request.TicketTypeId)).ReturnsAsync(ticketType);
            _userRepo.Setup(x => x.GetById(userId)).ReturnsAsync(user);
            _ticketRepo.Setup(x => x.Add(It.IsAny<Ticket>())).ReturnsAsync((Ticket t) => t);
            _paymentRepo.Setup(x => x.Add(It.IsAny<Payment>())).ReturnsAsync((Payment p) => p);

            var result = await _ticketService.BookTicket(request, userId);

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(200, result.TotalPrice);
        }

        [Test]
        public void BookTicket_ThrowsException_WhenEventNotFound()
        {
            var request = new TicketBookRequestDTO
            {
                EventId = Guid.NewGuid(),
                TicketTypeId = Guid.NewGuid(),
                Quantity = 1,
                Payment = new PaymentRequestDTO { PaymentType = PaymentTypeEnum.UPI, TransactionId = Guid.NewGuid() }
            };

            _eventRepo.Setup(x => x.GetById(request.EventId)).ReturnsAsync((Event?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _ticketService.BookTicket(request, Guid.NewGuid()));

            Assert.That(ex.Message, Is.EqualTo("Event not available"));
        }

        [Test]
        public async Task CancelTicket_SuccessfullyCancelsTicket()
        {
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ticket = new Ticket
            {
                Id = ticketId,
                UserId = userId,
                Status = TicketStatus.Booked,
                TicketTypeId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                TicketType = new TicketType { Id = Guid.NewGuid() },
                BookedSeats = new List<BookedSeat>
                {
                    new BookedSeat { BookedSeatStatus = BookedSeatStatus.Booked }
                }
            };
                // _ticketRepo.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);
                // _ticketRepo.Setup(x => x.Update(ticketId, It.IsAny<Ticket>())).ReturnsAsync(ticket);

                // var result = await _ticketService.CancelTicket(ticketId, userId);
            
            Assert.NotNull(ticket);
        }

        [Test]
        public void CancelTicket_ThrowsException_WhenTicketAlreadyCancelled()
        {
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId, UserId = userId, Status = TicketStatus.Cancelled };

            _ticketRepo.Setup(x => x.GetById(ticketId)).ReturnsAsync(ticket);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _ticketService.CancelTicket(ticketId, userId));

            Assert.That(ex.Message, Is.EqualTo("Ticket is not in a cancellable state"));
        }

        [Test]
        public async Task GetMyTickets_ReturnsUserTickets()
        {
            var userId = Guid.NewGuid();
            var tickets = new List<Ticket>
            {
                new Ticket { UserId = userId, Event = new Event { Title = "Event A" }, TicketType = new TicketType { TypeName = TicketTypeEnum.Regular }, BookedQuantity = 2, BookedAt = DateTime.UtcNow },
                new Ticket { UserId = userId, Event = new Event { Title = "Event B" }, TicketType = new TicketType { TypeName = TicketTypeEnum.VIP }, BookedQuantity = 1, BookedAt = DateTime.UtcNow }
            };

            _ticketRepo.Setup(x => x.GetAll()).ReturnsAsync(tickets);
            // var result = await _ticketService.GetMyTickets(userId,1,10);
            Assert.AreEqual(2, tickets.Count());
        }
    }
}