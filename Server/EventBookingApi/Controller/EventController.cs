using EventBookingApi.Interface;
using EventBookingApi.Model;
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
                return BadRequest(ApiResponse<object>.ErrorResponse("Event not found", new { ex.Message }));
            }
        }
        // [HttpPut("{eventId}/image")]
        // public async Task<IActionResult> UpdateEventImage(Guid eventId, IFormFile imageFile)
        // {
            

        //     try
        //     {
        //         if (imageFile == null || imageFile.Length == 0)
        //             throw new Exception("No image file uploaded.");
                
        //         var updatedEvent = await _eventService.UpdateEventImageUrl(eventId, imageFile);

        //         return Ok(ApiResponse<object>.SuccessResponse("Event image updated", updatedEvent));
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update image", new { ex.Message }));
        //     }
        // }
        // [HttpGet("image")]
        // public  IActionResult GetEventImage(string? imageUrl)
        // {
        //     if (string.IsNullOrWhiteSpace(imageUrl))
        //         return BadRequest("Image path is required");
        //     // /Users/presidio/Desktop/GenSpark_Training/DotNetWebApi_Project/EventBookingApi/wwwroot/uploads/
        //     // events/5a445565-e6b4-496a-a519-721cb5618e02.webp
        //     var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
        //     // System.Console.WriteLine(imagePath);
        //     if (!System.IO.File.Exists(imagePath))
        //         return NotFound("Image not found");

        //     var contentType = GetContentType(imagePath);
        //     var imageBytes = System.IO.File.ReadAllBytes(imagePath);

        //     return File(imageBytes, contentType);
        // }

        // private string GetContentType(string path)
        // {
        //     var ext = Path.GetExtension(path).ToLowerInvariant();
        //     return ext switch
        //     {
        //         ".jpg" or ".jpeg" => "image/jpeg",
        //         ".png" => "image/png",
        //         ".gif" => "image/gif",
        //         ".webp" => "image/webp", 
        //         _ => "application/octet-stream"
        //     };
        // }

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
        public async Task<IActionResult> FilterEvents([FromQuery]EventCategory? category, [FromQuery]Guid? cityId,[FromQuery] EventType? type,[FromQuery] string? searchElement, [FromQuery] DateTime? date,
                                                    [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var events = await _eventService.FilterEvents(category,cityId,type,searchElement!, date, pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("Filtered events fetched", events));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to filter events", new { ex.Message }));
            }
        }
        [HttpGet("cities")]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var events = await _eventService.getAllCities();
                return Ok(ApiResponse<object>.SuccessResponse("Cities Fetched", events));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Cities fetch failed", new { ex.Message }));
            }
        }
    }

}
