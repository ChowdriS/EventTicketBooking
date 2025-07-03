using System.Security.Claims;
using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingApi.Controller
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IOtherFunctionalities _otherFuntionailities;

        public TicketController(ITicketService ticketService,
                                IOtherFunctionalities otherFuntionailities)
        {
            _ticketService = ticketService;
            _otherFuntionailities = otherFuntionailities;
        }

        [HttpPost("book")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookTicket([FromBody] TicketBookRequestDTO dto)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var ticket = await _ticketService.BookTicket(dto, userId );
                return Ok(ApiResponse<object>.SuccessResponse("Ticket booked successfully", ticket));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to book ticket", new { ex.Message }));
            }
        }

        [HttpGet("mine")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyTickets([FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                // var tickets = await _ticketService.GetMyTickets(userId,pageNumber,  pageSize);
                var tickets = await _otherFuntionailities.GetPaginatedMyTickets(userId, pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("My tickets retrieved", tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to fetch tickets", new { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        // [Authorize(Roles = "User")]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var ticket = await _ticketService.GetTicketById(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse("Ticket fetched", ticket));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to get ticket", new { ex.Message }));
            }
        }

        [HttpDelete("{id}/cancel")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelTicket(Guid id)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var response = await _ticketService.CancelTicket(id, userId);
                return Ok(ApiResponse<object>.SuccessResponse("Ticket cancelled", response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to cancel ticket", new { ex.Message }));
            }
        }

        [HttpGet("{id}/export")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ExportTicket(Guid id)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                var file = await _ticketService.ExportTicketAsPdf(id, userId);
                // var createdFile = File(file, "application/pdf", $"Ticket_{id}.pdf");
                // return Ok(ApiResponse<object>.SuccessResponse("Ticket generatated", createdFile));
                return File(file, "application/pdf", $"Ticket_{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Export failed", new { ex.Message }));
            }
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetTicketsForEvent(Guid eventId,[FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                var userId = _otherFuntionailities.GetLoggedInUserId(User);
                // var tickets = await _ticketService.GetTicketsByEventId(eventId, userId, pageNumber, pageSize);
                var tickets = await _otherFuntionailities.GetPaginatedTicketsByEventId(eventId,userId, pageNumber, pageSize);
                return Ok(ApiResponse<object>.SuccessResponse("Tickets retrieved", tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to get tickets for event", new { ex.Message }));
            }
        }
    }
}
