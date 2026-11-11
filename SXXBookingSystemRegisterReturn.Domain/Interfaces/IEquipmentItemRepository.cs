using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Domain.Interfaces;

public interface IEquipmentItemRepository
{
    public Task<EquipmentItem> GetEquipmentItemByInventoryNumberAsync(string inventoryNumber);
    public Task<bool> UpdateAsync(EquipmentItem equipmentItem);
}
