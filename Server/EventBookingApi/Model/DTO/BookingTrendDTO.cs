using System;

namespace EventBookingApi.Model.DTO;

public class BookingTrendDTO
{
    public DateTime Date { get; set; }
    public int TotalBookings { get; set; }
}