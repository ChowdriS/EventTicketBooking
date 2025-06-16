using EventBookingApi.Interface;
using EventBookingApi.Model;
using EventBookingApi.Model.DTO;
using EventBookingApi.Service;
using EventBookingApi.Misc;
using Moq;

namespace EventBookingApi.Tests.ServiceTests
{
    [TestFixture]
    public class TicketTypeServiceTests
    {
        private Mock<IRepository<Guid, TicketType>> _ticketTypeRepoMock = null!;
        private Mock<IRepository<Guid, Event>> _eventRepoMock = null!;
        private Mock<ObjectMapper> _mapperMock = null!;
        private TicketTypeService _service = null!;
        private Guid _managerId;
        private Guid _eventId;

        [SetUp]
        public void Setup()
        {
            _ticketTypeRepoMock = new Mock<IRepository<Guid, TicketType>>();
            _eventRepoMock = new Mock<IRepository<Guid, Event>>();
            _mapperMock = new Mock<ObjectMapper>();
            _service = new TicketTypeService(_ticketTypeRepoMock.Object, _eventRepoMock.Object, _mapperMock.Object);

            _managerId = Guid.NewGuid();
            _eventId = Guid.NewGuid();
        }

        [Test]
        public async Task AddTicketType_Successful()
        {
            var dto = new TicketTypeAddRequestDTO
            {
                EventId = _eventId,
                Price = 100,
                TotalQuantity = 50,
                TypeName = TicketTypeEnum.Regular,
                Description = "Test type"
            };

            var ev = new Event { Id = _eventId, ManagerId = _managerId, IsDeleted = false };
            var ticketType = new TicketType { EventId = _eventId, Price = 100, TotalQuantity = 50 };
            var responseDto = new TicketTypeResponseDTO { EventId = _eventId };

            _eventRepoMock.Setup(r => r.GetById(_eventId)).ReturnsAsync(ev);
            _ticketTypeRepoMock.Setup(r => r.Add(It.IsAny<TicketType>())).ReturnsAsync(ticketType);
            _mapperMock.Setup(m => m.TicketTypeResponseDTOMapper(ticketType)).Returns(responseDto);

            var result = await _service.AddTicketType(_managerId, dto);

            Assert.That(result.EventId, Is.EqualTo(_eventId));
        }

        [Test]
        public void AddTicketType_EventDeleted_ThrowsException()
        {
            var dto = new TicketTypeAddRequestDTO { EventId = _eventId };
            var ev = new Event { Id = _eventId, IsDeleted = true };

            _eventRepoMock.Setup(r => r.GetById(_eventId)).ReturnsAsync(ev);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddTicketType(_managerId, dto));
            Assert.That(ex!.Message, Is.EqualTo("Associated event not found"));
        }

        [Test]
        public void AddTicketType_UnauthorizedManager_ThrowsException()
        {
            var dto = new TicketTypeAddRequestDTO { EventId = _eventId };
            var ev = new Event { Id = _eventId, ManagerId = Guid.NewGuid(), IsDeleted = false };

            _eventRepoMock.Setup(r => r.GetById(_eventId)).ReturnsAsync(ev);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.AddTicketType(_managerId, dto));
            Assert.That(ex!.Message, Is.EqualTo("Unauthorised Access"));
        }

        [Test]
        public async Task DeleteTicketType_SetsIsDeletedTrue()
        {
            var ticketTypeId = Guid.NewGuid();
            var ticketType = new TicketType { Id = ticketTypeId, EventId = _eventId };
            var ev = new Event { Id = _eventId, ManagerId = _managerId };
            var responseDto = new TicketTypeResponseDTO { Id = ticketTypeId, IsDeleted = true };

            _ticketTypeRepoMock.Setup(r => r.GetById(ticketTypeId)).ReturnsAsync(ticketType);
            _eventRepoMock.Setup(r => r.GetById(_eventId)).ReturnsAsync(ev);
            _ticketTypeRepoMock.Setup(r => r.Update(ticketTypeId, It.IsAny<TicketType>())).ReturnsAsync(ticketType);
            _mapperMock.Setup(m => m.TicketTypeResponseDTOMapper(ticketType)).Returns(responseDto);

            var result = await _service.DeleteTicketType(_managerId, ticketTypeId);

            Assert.That(result.IsDeleted, Is.True);
        }

        [Test]
        public void UpdateTicketType_LessThanBookedQuantity_ThrowsException()
        {
            var ticketTypeId = Guid.NewGuid();
            var ticketType = new TicketType
            {
                Id = ticketTypeId,
                EventId = _eventId,
                BookedQuantity = 30,
                TotalQuantity = 50
            };
            var ev = new Event { Id = _eventId, ManagerId = _managerId };

            var dto = new TicketTypeUpdateRequestDTO { TotalQuantity = 20 };

            _ticketTypeRepoMock.Setup(r => r.GetById(ticketTypeId)).ReturnsAsync(ticketType);
            _eventRepoMock.Setup(r => r.GetById(_eventId)).ReturnsAsync(ev);

            var ex = Assert.ThrowsAsync<Exception>(() => _service.UpdateTicketType(_managerId, ticketTypeId, dto));
            Assert.That(ex!.Message, Is.EqualTo("Total quantity cannot be less than booked quantity"));
        }
    }
}
