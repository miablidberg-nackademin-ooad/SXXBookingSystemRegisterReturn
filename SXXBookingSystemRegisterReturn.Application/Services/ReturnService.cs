using SXXBookingSystemRegisterReturn.Application.Dtos;
using SXXBookingSystemRegisterReturn.Domain.Helpers;
using SXXBookingSystemRegisterReturn.Domain.Interfaces;
using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Application.Services;

public class ReturnService(IReturnRepository returnRepository, IBookingRepository bookingRepository, IEquipmentItemRepository equipmentItemRepository) : IReturnService
{
    private readonly IReturnRepository _returnRepository = returnRepository;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IEquipmentItemRepository _equipmentItemRepository = equipmentItemRepository;

    public async Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId)
    {
        if (bookingId == Guid.Empty)
        {
            return new GetBookingResponse(false, null, $"Bokningen med id '{bookingId}' kunde inte hittas.");
        }

        Booking booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking is null)
        {
            return new GetBookingResponse(false, null, $"Bokningen med id '{bookingId}' kunde inte hittas.");
        }

        return new GetBookingResponse(true, booking, null);
    }

    public async Task<CheckInventoryNumberResponse> CheckInventoryNumberAsync(string inventoryNumber, Booking booking)
    {
        try
        {
            string normalizedInventoryNumber = Validation.ValidateAndNormalizeInventoryNumber(inventoryNumber);

            if (inventoryNumber.Equals(booking.InventoryNumber))
            {
                return new CheckInventoryNumberResponse(true, null);
            }
            else
            {
                return new CheckInventoryNumberResponse(false, "Fel inventarienummer på det återlämnade exemplaret jämfört med bokningen.");
            }
        }
        catch
        {
            return new CheckInventoryNumberResponse(false, "Inget angivet inventarienummer.");
        }
    }

    public async Task<RegisterReturnResponse> RegisterReturnAsync(Booking booking, string inventoryNumber, Guid staffId)
    {
        Return returnToAdd;

        try
        {
            returnToAdd = CreateReturnObject(booking, inventoryNumber, staffId);
            bool addSuccess = await _returnRepository.AddAsync(returnToAdd);

            if (!addSuccess)
            {
                return new RegisterReturnResponse(false, "Återlämningen kunde inte sparas.");
            }

            Booking updatedBooking = UpdateBooking(booking);
            bool bookingUpdateSuccess = await _bookingRepository.UpdateAsync(updatedBooking);

            if (!bookingUpdateSuccess)
            {
                return new RegisterReturnResponse(false, "Bokningen kunde inte uppdateras.");
            }

            EquipmentItem equipmentItem = await _equipmentItemRepository.GetEquipmentItemByInventoryNumberAsync(inventoryNumber);

            if (equipmentItem is null)
            {
                return new RegisterReturnResponse(false, $"Utrustningsexemplar med inventarienummer '{inventoryNumber}' kunde inte hittas.");
            }

            EquipmentItem updatedEquipmentItem = UpdateEquipmentItem(equipmentItem);
            bool equipmentItemUpdateSuccess = await _equipmentItemRepository.UpdateAsync(updatedEquipmentItem);

            if (!equipmentItemUpdateSuccess)
            {
                return new RegisterReturnResponse(false, "Utrustningsexemplaret kunde inte uppdateras.");
            }

            return new RegisterReturnResponse(true, null);
        }
        catch (Exception ex)
        {
            return new RegisterReturnResponse(false, ex.Message);
        }
    }

    private static Return CreateReturnObject(Booking booking, string inventoryNumber, Guid staffId)
    {
        Guid returnId = GenerateNewId();
        DateOnly returnDate = DateOnly.FromDateTime(DateTime.Now);

        Return returnToAdd = new(returnId, booking.BookingId, inventoryNumber, returnDate, staffId);

        return returnToAdd;
    }

    private static Guid GenerateNewId() => Guid.NewGuid();

    private static Booking UpdateBooking(Booking booking)
    {
        booking.Status = Domain.Enums.BookingStatus.Completed;
        booking.ReservedThrough = DateOnly.FromDateTime(DateTime.Now);

        return booking;
    }

    private static EquipmentItem UpdateEquipmentItem(EquipmentItem equipmentItem)
    {
        equipmentItem.IsAvailable = true;

        return equipmentItem;
    }
}