using System;

namespace EventBookingApi.Model.DTO;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(string message, T? data)
    {
        return new ApiResponse<T> { Success = true, Message = message, Data = data, Errors = null };
    }

    public static ApiResponse<T> ErrorResponse(string message, object? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Data = default, Errors = errors };
    }
}

