namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class EquipmentItem
{
    public string InventoryItem { get; set; }
    public string ArticleNumber { get; set; }
    public bool IsAvailable { get; set; }

    public EquipmentItem() { }

    public EquipmentItem(string inventoryItem, string articleNumber, bool isAvailable)
    {
        InventoryItem = inventoryItem;
        ArticleNumber = articleNumber;
        IsAvailable = isAvailable;
    }
}