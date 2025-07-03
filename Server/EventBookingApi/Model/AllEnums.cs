namespace EventBookingApi.Model;

public enum UserRole
{
    Admin,
    User,
    Manager
}

public enum EventCategory
{
    Music = 0,
    Tech = 1,
    Art = 2,
    Education = 3,
    Sports = 4,
    Business = 5,
    Festival = 6
}


public enum EventType
{
    Seatable = 0,
    NonSeatable = 1
}

public enum EventStatus
{
    Active = 0,
    Cancelled = 1,
    Completed = 2
}
public enum BookedSeatStatus
{
    Booked = 0,
    Cancelled = 1
}

public enum TicketStatus
{
    Booked = 0,
    Cancelled = 1,
    Used = 2
}

public enum TicketTypeEnum
{
    Regular = 0,
    VIP = 1,
    EarlyBird = 2
}

public enum PaymentTypeEnum
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    UPI = 3
}

public enum PaymentStatusEnum
{
    Paid = 0,
    Failed = 1,
    Pending = 2,
    Refund = 3
}
