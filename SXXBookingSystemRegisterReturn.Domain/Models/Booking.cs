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
    public BookingStatus Status { get; set; }
    public Guid CustomerId { get; set; }

    public Booking() { }

    public Booking(Guid bookingId, string bookingNumber, DateTime createdAt, DateOnly startDate, DateOnly endDate, DateOnly reservedThrough, decimal agreedDailyPrice, decimal agreedTotalPrice, BookingStatus status, Guid customerId)
    {
        BookingId = ValidateId(bookingId);
        BookingNumber = ValidateAndNormalizeBookingNumber(bookingNumber);
        CreatedAt = createdAt;
        StartDate = startDate;
        EndDate = ValidateEndDate(endDate);
        ReservedThrough = reservedThrough;
        AgreedDailyPrice = ValidatePrice(agreedDailyPrice);
        AgreedTotalPrice = ValidatePrice(agreedTotalPrice);
        Status = ValidateStatus(status);
        CustomerId = ValidateId(customerId);
    }

    private static Guid ValidateId(Guid bookingId)
    {
        if (bookingId == Guid.Empty)
        {
            throw new ArgumentException("Bokningen måste ha ett ID.", nameof(bookingId));
        }

        return bookingId;
    }

    private static string ValidateAndNormalizeBookingNumber(string bookingNumber)
    {
        if (!string.IsNullOrWhiteSpace(bookingNumber))
        {
            throw new ArgumentException("Bokningen måste ha ett bokningsnummer.", nameof(bookingNumber));
        }

        string normalizedBookingNumber = bookingNumber.Trim();

        return normalizedBookingNumber;
    }

    private DateOnly ValidateEndDate(DateOnly endDate)
    {
        if (endDate <= StartDate)
        {
            throw new ArgumentException("Slutdatumet kan inte vara innan startdatumet.", nameof(endDate));
        }

        return endDate;
    }

    private decimal ValidatePrice(decimal agreedPrice)
    {
        if (agreedPrice < 0)
        {
            throw new ArgumentException("Priset kan inte vara negativt.", nameof(agreedPrice));
        }

        return agreedPrice;
    }

    private BookingStatus ValidateStatus(BookingStatus status)
    {
        if (Enum.IsDefined(status))
        {
            throw new ArgumentException("Ogiltig status för bokningen.", nameof(status));
        }

        return status;
    }
}