namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class Return
{
    public Guid ReturnId { get; set; }
    public Guid BookingId { get; set; }
    public string InventoryNumber { get; set; }
    public DateOnly ReturnDate { get; set; }
    public Guid StaffId { get; set; }

    public Return() { }

    public Return(Guid returnId, Guid bookingId, string inventoryNumber, DateOnly returnDate, Guid staffId)
    {
        ReturnId = returnId;
        BookingId = bookingId;
        InventoryNumber = inventoryNumber;
        ReturnDate = returnDate;
        StaffId = staffId;
    }
}