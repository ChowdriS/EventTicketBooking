using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingApi.Controller
{
    [Route("api/v1/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IOtherFunctionalities _otherFuntionailities;

        public EventController(IEventService eventService, IOtherFunctionalities otherFuntionailities)
        {
            _eventService = eventService;
            _otherFuntionailities = otherFuntionailities;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var events = await _eventService.GetAllEvents(pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("Events fetched successfully", events));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to fetch events", new { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            try
            {
                var evt = await _eventService.GetEventById(id);
                return Ok(ApiResponse<object>.SuccessResponse("Event fetched successfully", evt));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Event not found", new { ex.Message }));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateEvent(EventAddRequestDTO dto)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var evt = await _eventService.AddEvent(dto,userId);
                return Ok(ApiResponse<object>.SuccessResponse("Event created successfully", evt));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Event creation failed", new { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateEvent(Guid id, EventUpdateRequestDTO dto)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var updatedEvent = await _eventService.UpdateEvent(id, dto);
                return Ok(ApiResponse<object>.SuccessResponse("Event updated successfully", updatedEvent));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Event update failed", new { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var result = await _eventService.DeleteEvent(id);
                return Ok(ApiResponse<object>.SuccessResponse("Event deleted successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Event deletion failed", new { ex.Message }));
            }
        }

        [HttpGet("myevents")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetMyManagedEvents([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var events = await _eventService.GetManagedEventsByUserId(userId, pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("Managed events fetched", events));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to fetch managed events", new { ex.Message }));
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterEvents([FromQuery] string searchElement, [FromQuery] DateTime? date,
                                                    [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var events = await _eventService.FilterEvents(searchElement, date, pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("Filtered events fetched", events));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to filter events", new { ex.Message }));
            }
        }
    }

}
