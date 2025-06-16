using Moq;
using EventBookingApi.Service;
using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Misc;

namespace EventBookingTest.Test.ServiceTest;
public class PaymentServiceTests
{
    private Mock<IRepository<Guid, Payment>> _mockPaymentRepo;
    private Mock<IRepository<Guid, Ticket>> _mockTicketRepo;
    private Mock<IRepository<Guid, Event>> _mockEventRepo;
    private Mock<IRepository<Guid, User>> _mockUserRepo;
    private PaymentService _service;
    private Mock<ObjectMapper> _mockMapper;

    [SetUp]
    public void Setup()
    {
        _mockPaymentRepo = new Mock<IRepository<Guid, Payment>>();
        _mockTicketRepo = new Mock<IRepository<Guid, Ticket>>();
        _mockEventRepo = new Mock<IRepository<Guid, Event>>();
        _mockUserRepo = new Mock<IRepository<Guid, User>>();

        _mockMapper = new Mock<ObjectMapper>();
        _service = new PaymentService(
            _mockPaymentRepo.Object,
            _mockTicketRepo.Object,
            _mockEventRepo.Object,
            _mockUserRepo.Object,
            _mockMapper.Object);
            }

    [Test]
    public async Task GetPaymentById_ReturnsPaymentDetailDTO_WhenAuthorized()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var payment = new Payment
        {
            Id = paymentId,
            TicketId = ticketId,
            Amount = 150m,
            PaymentStatus = PaymentStatusEnum.Paid,
            PaymentType = PaymentTypeEnum.UPI,
            TransactionId = Guid.NewGuid(),
            PaidAt = DateTime.UtcNow
        };

        var ticket = new Ticket
        {
            Id = ticketId,
            UserId = userId,
            EventId = eventId,
            BookedAt = DateTime.UtcNow,
            Status = TicketStatus.Booked
        };

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            Role = UserRole.User
        };

        var eventObj = new Event
        {
            Id = eventId,
            Title = "Test Event"
        };

        _mockPaymentRepo.Setup(r => r.GetById(paymentId)).ReturnsAsync(payment);
        _mockTicketRepo.Setup(r => r.GetById(ticketId)).ReturnsAsync(ticket);
        _mockUserRepo.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _mockEventRepo.Setup(r => r.GetById(eventId)).ReturnsAsync(eventObj);

        // Act
        var result = await _service.GetPaymentById(paymentId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(payment.Id));
        Assert.That(eventObj.Title, Is.EqualTo(result.EventTitle));
        Assert.That(user.Username, Is.EqualTo(result.UserName));
        Assert.That(ticket.Status, Is.EqualTo(result.TicketStatus));
    }

    [Test]
    public void GetPaymentById_ThrowsUnauthorizedAccessException_WhenUserNotOwner()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();

        var payment = new Payment { Id = paymentId, TicketId = ticketId };
        var ticket = new Ticket { Id = ticketId, UserId = otherUserId };
        var user = new User { Id = userId, Role = UserRole.User };

        _mockPaymentRepo.Setup(r => r.GetById(paymentId)).ReturnsAsync(payment);
        _mockTicketRepo.Setup(r => r.GetById(ticketId)).ReturnsAsync(ticket);
        _mockUserRepo.Setup(r => r.GetById(userId)).ReturnsAsync(user);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetPaymentById(paymentId, userId));
    }

    [Test]
    public async Task GetPaymentsByUserId_ReturnsPaymentsForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var tickets = new List<Ticket>
        {
            new Ticket
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventId = eventId,
                BookedAt = DateTime.UtcNow,
                Status = TicketStatus.Booked
            },
            new Ticket
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                EventId = eventId,
                BookedAt = DateTime.UtcNow,
                Status = TicketStatus.Booked
            }
        };

        var payments = new List<Payment>
        {
            new Payment { Id = Guid.NewGuid(), TicketId = tickets[0].Id, Amount = 100, PaymentStatus = PaymentStatusEnum.Paid },
            new Payment { Id = Guid.NewGuid(), TicketId = tickets[1].Id, Amount = 50, PaymentStatus = PaymentStatusEnum.Paid }
        };

        var events = new List<Event>
        {
            new Event { Id = eventId, Title = "Event 1" }
        };

        var user = new User { Id = userId, Username = "User1", Email = "user1@example.com" };

        _mockTicketRepo.Setup(r => r.GetAll()).ReturnsAsync(tickets);
        _mockPaymentRepo.Setup(r => r.GetAll()).ReturnsAsync(payments);
        _mockEventRepo.Setup(r => r.GetAll()).ReturnsAsync(events);
        _mockUserRepo.Setup(r => r.GetById(userId)).ReturnsAsync(user);

        // Act
        var results = await _service.GetPaymentsByUserId(userId);

        // Assert
        Assert.That(results.Any());
        Assert.That(results.All(p => p.UserId == userId));
    }

    [Test]
    public async Task GetPaymentsByEventId_ReturnsPayments_WhenManagerAuthorized()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var eventObj = new Event
        {
            Id = eventId,
            Title = "Managed Event",
            ManagerId = managerId
        };

        var tickets = new List<Ticket>
        {
            new Ticket { Id = Guid.NewGuid(), EventId = eventId, UserId = userId, BookedAt = DateTime.UtcNow, Status = TicketStatus.Booked },
            new Ticket { Id = Guid.NewGuid(), EventId = eventId, UserId = userId, BookedAt = DateTime.UtcNow, Status = TicketStatus.Booked }
        };

        var payments = new List<Payment>
        {
            new Payment { Id = Guid.NewGuid(), TicketId = tickets[0].Id, Amount = 200, PaymentStatus = PaymentStatusEnum.Paid },
            new Payment { Id = Guid.NewGuid(), TicketId = tickets[1].Id, Amount = 100, PaymentStatus = PaymentStatusEnum.Paid }
        };

        var users = new List<User>
        {
            new User { Id = userId, Username = "UserX", Email = "userx@example.com" }
        };

        _mockEventRepo.Setup(r => r.GetById(eventId)).ReturnsAsync(eventObj);
        _mockTicketRepo.Setup(r => r.GetAll()).ReturnsAsync(tickets);
        _mockPaymentRepo.Setup(r => r.GetAll()).ReturnsAsync(payments);
        _mockUserRepo.Setup(r => r.GetAll()).ReturnsAsync(users);

        // Act
        var results = await _service.GetPaymentsByEventId(eventId, managerId);

        // Assert
        Assert.That(results.Any());
        Assert.That(results.All(p => p.EventId == eventId));
    }

    [Test]
    public void GetPaymentsByEventId_ThrowsUnauthorizedAccessException_WhenManagerMismatch()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var managerId = Guid.NewGuid();

        var eventObj = new Event { Id = eventId, ManagerId = Guid.NewGuid() }; 

        _mockEventRepo.Setup(r => r.GetById(eventId)).ReturnsAsync(eventObj);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetPaymentsByEventId(eventId, managerId));
    }
}
