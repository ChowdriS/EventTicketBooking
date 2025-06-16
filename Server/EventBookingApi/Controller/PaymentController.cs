using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingApi.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOtherFunctionalities _otherFunctionalities;

        public PaymentController(
            IPaymentService paymentService,
            IOtherFunctionalities otherFunctionalities)
        {
            _paymentService = paymentService;
            _otherFunctionalities = otherFunctionalities;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<PaymentDetailDTO>> GetPaymentById(Guid id)
        {
            try
            {
                var currentUserId = _otherFunctionalities.GetLoggedInUserId(User);
                var payment = await _paymentService.GetPaymentById(id, currentUserId);
                return Ok(ApiResponse<object>.SuccessResponse("Payment successfully fetched", payment));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Payment fetch Failed", new { ex.Message }));
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PaymentDetailDTO>>> GetPaymentsByUser(Guid userId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByUserId(userId);
                return Ok(ApiResponse<object>.SuccessResponse("Payment successfully fetched", payments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Payment fetch Failed", new { ex.Message }));
            }
        }
        [HttpGet("ticket/{tickedId}")]
        public async Task<ActionResult<PaymentDetailDTO>> GetPaymentsByTicketId(Guid tickedId)
        {
            try
            {
                var userId = _otherFunctionalities.GetLoggedInUserId(User);
                var payment = await _paymentService.GetPaymentByTicketId(tickedId, userId);
                return Ok(ApiResponse<object>.SuccessResponse("Payment successfully fetched", payment));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Payment fetch Failed", new { ex.Message }));
                
            }
        }
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<PaymentDetailDTO>>> GetPaymentsByEvent(Guid eventId)
        {
            try
            {
                Guid? managerId = null;
                if (User.IsInRole("Manager"))
                {
                    managerId = _otherFunctionalities.GetLoggedInUserId(User);
                }
                var payments = await _paymentService.GetPaymentsByEventId(eventId, managerId);
                return Ok(ApiResponse<object>.SuccessResponse("Payment successfully fetched", payments));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Payment fetch Failed", new { ex.Message }));

            }
        }
    }
}
