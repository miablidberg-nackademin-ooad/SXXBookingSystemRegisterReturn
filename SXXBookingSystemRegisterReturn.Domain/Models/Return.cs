using SXXBookingSystemRegisterReturn.Domain.Helpers;

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
        ReturnId = Validation.ValidateId(returnId, "återlämningen");
        BookingId = Validation.ValidateId(bookingId, "bokningen");
        InventoryNumber = Validation.ValidateAndNormalizeInventoryNumber(inventoryNumber);
        ReturnDate = returnDate;
        StaffId = Validation.ValidateId(staffId, "registrerande personal");
    }
}