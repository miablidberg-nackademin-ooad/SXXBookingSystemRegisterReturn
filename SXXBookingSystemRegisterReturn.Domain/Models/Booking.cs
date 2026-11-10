using SXXBookingSystemRegisterReturn.Domain.Enums;

namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class Booking
{
    public Guid BookingId { get; private set; }
    public string BookingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly ReservedThrough { get; set; }
    public decimal AgreedDailyPrice { get; set; }
    public decimal AgreedTotalPrice { get; set; }
    public string Currency { get; set; }
    public BookingStatus Status { get; set; }
    public Guid CustomerId { get; set; }

    public Booking() { }

    public Booking(Guid bookingId, string bookingNumber, DateTime createdAt, DateOnly startDate, DateOnly endDate, DateOnly reservedThrough, decimal agreedDailyPrice, decimal agreedTotalPrice, string currency, BookingStatus status, Guid customerId)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
        CreatedAt = createdAt;
        StartDate = startDate;
        EndDate = endDate;
        ReservedThrough = reservedThrough;
        AgreedDailyPrice = agreedDailyPrice;
        AgreedTotalPrice = agreedTotalPrice;
        Currency = currency;
        Status = status;
        CustomerId = customerId;
    }
}
