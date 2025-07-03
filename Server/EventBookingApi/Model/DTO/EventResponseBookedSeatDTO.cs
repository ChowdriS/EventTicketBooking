using System;

namespace EventBookingApi.Model.DTO;

public class EventResponseBookedSeatDTO
{
    public int SeatNumber { get; set; }

    public BookedSeatStatus BookedSeatStatus { get; set; }
}
