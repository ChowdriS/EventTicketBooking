using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using Moq;
using NUnit.Framework;

namespace EventBookingApi.Test.ServiceTest;

public class EventServiceTest
{
    private Mock<IRepository<Guid, Event>> _eventRepoMock;
    private Mock<IRepository<Guid, TicketType>> _ticketTypeRepoMock;
    private Mock<IRepository<Guid, User>> _userRepoMock;
    private Mock<IOtherFunctionalities> _otherFuncMock;
    private EventService _eventService;
    private ObjectMapper _realMapper;

    [SetUp]
    public void Setup()
    {
        _eventRepoMock = new Mock<IRepository<Guid, Event>>();
        _ticketTypeRepoMock = new Mock<IRepository<Guid, TicketType>>();
        _userRepoMock = new Mock<IRepository<Guid, User>>();
        _otherFuncMock = new Mock<IOtherFunctionalities>();
        _realMapper = new ObjectMapper();

        _eventService = new EventService(
            _eventRepoMock.Object,
            _ticketTypeRepoMock.Object,
            _userRepoMock.Object,
            _otherFuncMock.Object,
            _realMapper 
        );
    }

    [Test]
    public async Task GetAllEvents_ReturnsEvents()
    {
        var eventList = new List<EventResponseDTO>
        {
            new EventResponseDTO { Title = "Test Event" }
        };
        var paginated = new PaginatedResultDTO<EventResponseDTO>
        {
            Items = eventList,
            PageNumber = 1,
            PageSize = 10,
            TotalItems = 1
        };

        _otherFuncMock.Setup(f => f.GetPaginatedEvents(1, 10)).ReturnsAsync(paginated);

        var result = await _eventService.GetAllEvents(1, 10);

        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Title, Is.EqualTo("Test Event"));
    }

    [Test]
    public async Task GetEventById_ValidId_ReturnsEvent()
    {
        var id = Guid.NewGuid();
        var ev = new Event { Id = id, Title = "Event", IsDeleted = false };

        _eventRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(ev);

        var result = await _eventService.GetEventById(id);

        Assert.That(result.Title, Is.EqualTo("Event"));
    }

    [Test]
    public void GetEventById_InvalidId_ThrowsException()
    {
        _eventRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((Event)null);

        Assert.ThrowsAsync<Exception>(() => _eventService.GetEventById(Guid.NewGuid()));
    }

    [Test]
    public async Task FilterEvents_ByCity_ReturnsMatchingEvents()
    {
        var paginated = new PaginatedResultDTO<EventResponseDTO>
        {
            Items = new List<EventResponseDTO>
            {
                new EventResponseDTO { Description = "Chennai" }
            },
            PageNumber = 1,
            PageSize = 10,
            TotalItems = 1
        };

        _otherFuncMock.Setup(f => f.GetPaginatedEventsByFilter("Chennai", null, 1, 10))
                      .ReturnsAsync(paginated);

        var result = await _eventService.FilterEvents("Chennai", null, 1, 10);

        Assert.That(result.Items.Count, Is.EqualTo(1));
        Assert.That(result.Items.First().Description, Is.EqualTo("Chennai"));
    }

    [Test]
    public async Task GetManagedEventsByUserId_ReturnsEvents()
    {
        var userId = Guid.NewGuid();

        var paginated = new PaginatedResultDTO<EventResponseDTO>
        {
            Items = new List<EventResponseDTO> { new EventResponseDTO { Title = "Managed" } },
            PageNumber = 1,
            PageSize = 10,
            TotalItems = 1
        };

        _otherFuncMock.Setup(f => f.GetPaginatedEventsByManager(userId, 1, 10))
                      .ReturnsAsync(paginated);

        var result = await _eventService.GetManagedEventsByUserId(userId, 1, 10);

        Assert.That(result.Items.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task AddEvent_WithTickets_AddsEventAndTickets()
    {
        var managerId = Guid.NewGuid();
        var newEventId = Guid.NewGuid();

        var dto = new EventAddRequestDTO
        {
            Title = "New Event",
            Description = "Test Desc",
            EventDate = DateTime.Today,
            EventType = EventType.Seatable,
            TicketTypes = new List<TicketTypeAddRequestDTO>
            {
                new TicketTypeAddRequestDTO
                {
                    TypeName = TicketTypeEnum.VIP,
                    Price = 100,
                    TotalQuantity = 10,
                    Description = "VIP"
                }
            }
        };

        var newEvent = new Event
        {
            Id = newEventId,
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            EventType = dto.EventType,
            ManagerId = managerId
        };

        _userRepoMock.Setup(u => u.GetById(managerId)).ReturnsAsync(new User { Id = managerId });
        _eventRepoMock.Setup(e => e.Add(It.IsAny<Event>()))
                      .ReturnsAsync(newEvent);

        var result = await _eventService.AddEvent(dto, managerId);

        _ticketTypeRepoMock.Verify(t => t.Add(It.IsAny<TicketType>()), Times.Once);
        Assert.That(result.Title, Is.EqualTo("New Event"));
    }

    [Test]
    public async Task UpdateEvent_ValidUpdate_ChangesFields()
    {
        var eventId = Guid.NewGuid();
        var ev = new Event
        {
            Id = eventId,
            Title = "Old Title",
            Description = "Old",
            EventDate = DateTime.Today,
            EventType = EventType.Seatable,
            EventStatus = EventStatus.Active
        };

        var dto = new EventUpdateRequestDTO
        {
            Title = "Updated Title",
            Description = "Updated",
            EventDate = DateTime.Today.AddDays(1),
            EventType = EventType.NonSeatable,
            EventStatus = EventStatus.Completed
        };

        _eventRepoMock.Setup(r => r.GetById(eventId)).ReturnsAsync(ev);
        _eventRepoMock.Setup(r => r.Update(eventId, It.IsAny<Event>())).ReturnsAsync(ev);

        var result = await _eventService.UpdateEvent(eventId, dto);

        Assert.That(result.Title, Is.EqualTo("Updated Title"));
        Assert.That(result.EventStatus, Is.EqualTo("Completed"));
    }

    [Test]
    public async Task DeleteEvent_ValidId_SetsIsDeletedAndCancels()
    {
        var eventId = Guid.NewGuid();
        var ev = new Event { 
            Id = eventId, 
            Title = "ToDelete", 
            IsDeleted = false,
            EventStatus = EventStatus.Active
        };

        _eventRepoMock.Setup(r => r.GetById(eventId)).ReturnsAsync(ev);
        _eventRepoMock.Setup(r => r.Update(eventId, It.IsAny<Event>())).ReturnsAsync(ev);

        var result = await _eventService.DeleteEvent(eventId);

        Assert.That(result.EventStatus, Is.EqualTo("Cancelled"));
    }
}