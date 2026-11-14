using SXXBookingSystemRegisterReturn.Application.Dtos;
using SXXBookingSystemRegisterReturn.Domain.Enums;
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
            return new GetBookingResponse(false, null, $"Ange ett bokningsnummer.");
        }

        Booking? booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking is null)
        {
            return new GetBookingResponse(false, null, $"Bokningen med id '{bookingId}' kunde inte hittas.");
        }

        return new GetBookingResponse(true, booking, null);
    }

    public CheckInventoryNumberResponse CheckInventoryNumber(string inventoryNumber, Booking booking)
    {
        try
        {
            string normalizedInventoryNumber = Validation.ValidateAndNormalizeInventoryNumber(inventoryNumber);

            if (normalizedInventoryNumber.Equals(booking.InventoryNumber))
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

    public async Task<RegisterReturnResponse> RegisterReturnAsync(Booking booking, string inventoryNumber, Guid staffId, bool isAvailable)
    {
        Return returnToAdd;

        if (booking.Status != BookingStatus.Confirmed)
        {
            return new RegisterReturnResponse(false, "Uthyrningen är redan avslutad.");
        }
        else if (!CheckInventoryNumber(inventoryNumber, booking).Success)
        {
            return new RegisterReturnResponse(false, "Inventarienumret stämmer inte med bokningens inventarienummer.");
        }

        try
        {
            returnToAdd = CreateReturnObject(booking, inventoryNumber, staffId);
            bool addSuccess = await _returnRepository.AddAsync(returnToAdd);

            if (!addSuccess)
            {
                return new RegisterReturnResponse(false, "Registreringen av återlämningen misslyckades.");
            }

            Booking updatedBooking = UpdateBooking(booking);
            bool bookingUpdateSuccess = await _bookingRepository.UpdateAsync(updatedBooking);

            if (!bookingUpdateSuccess)
            {
                return new RegisterReturnResponse(false, "Uppdateringen av bokningen misslyckades.");
            }

            EquipmentItem equipmentItem = await _equipmentItemRepository.GetEquipmentItemByInventoryNumberAsync(inventoryNumber);

            if (equipmentItem is null)
            {
                return new RegisterReturnResponse(false, $"Utrustningsexemplar med inventarienummer '{inventoryNumber}' kunde inte hittas.");
            }

            EquipmentItem updatedEquipmentItem = UpdateEquipmentItem(equipmentItem, isAvailable);
            bool equipmentItemUpdateSuccess = await _equipmentItemRepository.UpdateAsync(updatedEquipmentItem);

            if (!equipmentItemUpdateSuccess)
            {
                return new RegisterReturnResponse(false, "Uppdateringen av utrustningsexemplaret misslyckades.");
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

    private static EquipmentItem UpdateEquipmentItem(EquipmentItem equipmentItem, bool isAvailable)
    {
        equipmentItem.IsAvailable = isAvailable;

        return equipmentItem;
    }
}