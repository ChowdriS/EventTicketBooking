enum UserRole
{
    Admin,
    User,
    Manager
}

enum EventType
{
    Seatable = 0,
    NonSeatable = 1
}

enum EventStatus
{
    Active = 0,
    Cancelled = 1,
    Completed = 2
}
enum BookedSeatStatus
{
    Booked = 0,
    Cancelled = 1
}

enum TicketStatus
{
    Booked = 0,
    Cancelled = 1,
    Used = 2
}

enum TicketTypeEnum
{
    Regular = 0,
    VIP = 1,
    EarlyBird = 2
}

enum PaymentTypeEnum
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    UPI = 3
}

enum PaymentStatusEnum
{
    Paid = 0,
    Failed = 1,
    Pending = 2,
    Refund = 3
}
