using SXXBookingSystemRegisterReturn.Domain.Enums;
using SXXBookingSystemRegisterReturn.Domain.Helpers;

namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class Booking
{
    public Guid BookingId { get; private set; }
    public string BookingNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public DateOnly ReservedThrough { get; set; }
    public decimal AgreedDailyPrice { get; private set; }
    public decimal AgreedTotalPrice { get; private set; }
    public string InventoryNumber { get; private set; }
    public BookingStatus Status { get; set; }
    public Guid CustomerId { get; private set; }

    public Booking() { }

    public Booking(Guid bookingId, string bookingNumber, DateTime createdAt, DateOnly startDate, 
        DateOnly endDate, DateOnly reservedThrough, decimal agreedDailyPrice, decimal agreedTotalPrice, 
        string inventoryNumber, BookingStatus status, Guid customerId)
    {
        BookingId = Validation.ValidateId(bookingId, "bokningen");
        BookingNumber = ValidateAndNormalizeBookingNumber(bookingNumber);
        CreatedAt = createdAt;
        StartDate = startDate;
        EndDate = ValidateEndDate(endDate);
        ReservedThrough = reservedThrough;
        AgreedDailyPrice = ValidatePrice(agreedDailyPrice);
        AgreedTotalPrice = ValidatePrice(agreedTotalPrice);
        InventoryNumber = Validation.ValidateAndNormalizeInventoryNumber(inventoryNumber);
        Status = ValidateStatus(status);
        CustomerId = Validation.ValidateId(customerId, "kunden");
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

    private static decimal ValidatePrice(decimal agreedPrice)
    {
        if (agreedPrice < 0)
        {
            throw new ArgumentException("Priset kan inte vara negativt.", nameof(agreedPrice));
        }

        return agreedPrice;
    }

    private static BookingStatus ValidateStatus(BookingStatus status)
    {
        if (Enum.IsDefined(status))
        {
            throw new ArgumentException("Ogiltig status för bokningen.", nameof(status));
        }

        return status;
    }
}