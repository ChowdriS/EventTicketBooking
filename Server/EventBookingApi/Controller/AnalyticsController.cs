using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventBookingApi.Interface;
using EventBookingApi.Model.DTO;
using Microsoft.AspNetCore.Authorization;

namespace EventBookingApi.Controller;

[ApiController]
[Route("api/v1/analytics")]
[Authorize]
public class AnalyticController : ControllerBase
{
    private readonly IAnalyticsService _analyticService;
    private readonly IOtherFunctionalities _otherFunctionalities;

    public AnalyticController(IAnalyticsService analyticService, IOtherFunctionalities otherFunctionalities)
    {
        _analyticService = analyticService;
        _otherFunctionalities = otherFunctionalities;
    }

    

    [HttpGet("booking-trends")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> GetBookingTrends()
    {
        try
        {
            var trends = await _analyticService.GetBookingTrends();
            return Ok(ApiResponse<object>.SuccessResponse("Booking trends retrieved successfully", trends));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Getting your details operation failed!", new { ex.Message }));
        }
    }

    [HttpGet("my-earnings")]
    [Authorize(Roles ="Manager")]

    public async Task<IActionResult> GetTotalEarnings()
    {
        try
        {
            var userId = _otherFunctionalities.GetLoggedInUserId(User);
            var data = await _analyticService.GetTotalEarnings(userId);
            return Ok(ApiResponse<object>.SuccessResponse("Total Earning fetched Successfully!", data));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Getting your details operation failed!", new { ex.Message }));
        }
    }
    [HttpGet("top-events")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> GetTopEvents()
    {
        try
        {
            var topEvents = await _analyticService.GetTopEvents();
            return Ok(ApiResponse<object>.SuccessResponse("Top events retrieved successfully", topEvents));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Getting your details operation failed!", new { ex.Message }));
        }
    }
}
