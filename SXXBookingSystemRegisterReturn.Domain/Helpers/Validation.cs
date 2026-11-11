namespace SXXBookingSystemRegisterReturn.Domain.Helpers;

public class Validation
{
    public static string ValidateAndNormalizeInventoryNumber(string inventoryNumber)
    {
        if (!string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Inventarienummer måste finnas.", nameof(inventoryNumber));
        }

        string normalizedInventoryNumber = inventoryNumber.Trim();

        return normalizedInventoryNumber;
    }

    public static Guid ValidateId(Guid id, string value)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException($"Det måste finnas ett ID för {value}.", nameof(id));
        }

        return id;
    }
}
