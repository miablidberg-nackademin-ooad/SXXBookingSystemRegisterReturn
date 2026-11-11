using SXXBookingSystemRegisterReturn.Domain.Helpers;

namespace SXXBookingSystemRegisterReturn.Domain.Models;

public class EquipmentItem
{
    public string InventoryNumber { get; private set; }
    public string ArticleNumber { get; private set; }
    public bool IsAvailable { get; set; }

    public EquipmentItem() { }

    public EquipmentItem(string inventoryNumber, string articleNumber, bool isAvailable)
    {
        InventoryNumber = Validation.ValidateAndNormalizeInventoryNumber(inventoryNumber);
        ArticleNumber = ValidateAndNormalizeArticleNumber(articleNumber);
        IsAvailable = isAvailable;
    }

    private static string ValidateAndNormalizeArticleNumber(string articleNumber)
    {
        if (!string.IsNullOrWhiteSpace(articleNumber))
        {
            throw new ArgumentException("Artikelnummer måste finnas.", nameof(articleNumber));
        }

        string normalizedInventoryNumber = articleNumber.Trim();

        return normalizedInventoryNumber;
    }
}