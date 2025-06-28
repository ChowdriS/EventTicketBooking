export enum UserRole
{
    Admin,
    User,
    Manager
}

export enum EventCategory
{
    Music = 0,
    Tech = 1,
    Art = 2,
    Education = 3,
    Sports = 4,
    Business = 5,
    Festival = 6
}



export enum EventTypeEnum
{
    Seatable = 0,
    NonSeatable = 1
}

export enum EventStatus
{
    Active = 0,
    Cancelled = 1,
    Completed = 2
}
export enum BookedSeatStatus
{
    Booked = 0,
    Cancelled = 1
}

export enum TicketStatus
{
    Booked = 0,
    Cancelled = 1,
    Used = 2
}

export enum TicketTypeEnum
{
    Regular = 0,
    VIP = 1,
    EarlyBird = 2
}

export enum PaymentTypeEnum
{
    Cash = 0,
    CreditCard = 1,
    DebitCard = 2,
    UPI = 3
}

export enum PaymentStatusEnum
{
    Paid = 0,
    Failed = 1,
    Pending = 2,
    Refund = 3
}
