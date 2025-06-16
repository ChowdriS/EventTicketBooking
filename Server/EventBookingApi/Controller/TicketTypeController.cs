using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingApi.Controller;

[ApiController]
[Route("api/v1/tickettype")]
public class TicketTypeController : ControllerBase
{
    private readonly ITicketTypeService _ticketTypeService;
    private readonly IOtherFunctionalities _otherFunctionalities;

    public TicketTypeController(ITicketTypeService ticketTypeService,
                                IOtherFunctionalities otherFunctionalities)
    {
        _ticketTypeService = ticketTypeService;
        _otherFunctionalities = otherFunctionalities;
    }

    [HttpGet("event/{eventId}")]
    [Authorize]
    public async Task<IActionResult> GetAllTicketTypesForEvent(Guid eventId)
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var ticketTypes = await _ticketTypeService.GetAllTicketTypesForEvent(userId, eventId);
            return Ok(ApiResponse<object>.SuccessResponse("Ticket types fetched successfully", ticketTypes));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to fetch ticket types", new { ex.Message }));
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetTicketTypeById(Guid id)
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var ticketType = await _ticketTypeService.GetTicketTypeById(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse("Ticket type fetched successfully", ticketType));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to fetch ticket type", new { ex.Message }));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AddTicketType([FromBody] TicketTypeAddRequestDTO dto)
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var ticketType = await _ticketTypeService.AddTicketType(userId,dto);
            return Ok(ApiResponse<object>.SuccessResponse("Ticket type added successfully", ticketType));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to add ticket type", new { ex.Message }));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    
    public async Task<IActionResult> UpdateTicketType(Guid id, [FromBody] TicketTypeUpdateRequestDTO dto)
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var ticketType = await _ticketTypeService.UpdateTicketType(userId, id, dto);
            return Ok(ApiResponse<object>.SuccessResponse("Ticket type updated successfully", ticketType));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update ticket type", new { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    
    public async Task<IActionResult> DeleteTicketType(Guid id)
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var ticketType = await _ticketTypeService.DeleteTicketType(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse("Ticket type deleted successfully", ticketType));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete ticket type", new { ex.Message }));
        }
    }
}
