using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class InventoryDTO
{
    [JsonPropertyName("inventoryId")]
    public int InventoryId { get; set; }
    [JsonPropertyName("productId")]
    public int? ProductId { get; set; }
    [JsonPropertyName("unitPrice")]
    public decimal? UnitPrice { get; set; }
    [JsonPropertyName("unitsInStock")]
    public int? UnitsInStock { get; set; }
    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; set; }
    [JsonPropertyName("lastUpdated")]
    public DateTime? LastUpdated { get; set; }
    [JsonPropertyName("modifiedBy")]
    public string? ModifiedBy { get; set; }

    public static InventoryDTO ConvertFrom(Inventory inventory)
    {
        return new InventoryDTO
        {
            InventoryId = inventory.InventoryId,
            ProductId = inventory.ProductId,
            UnitPrice = inventory.UnitPrice,
            UnitsInStock = inventory.UnitsInStock,
            DateAdded = inventory.DateAdded,
            LastUpdated = inventory.LastUpdated,
            ModifiedBy = inventory.ModifiedBy
        };
    }

    public static Inventory ConvertTo(InventoryDTO dto)
    {
        return new Inventory
        {
            InventoryId = dto.InventoryId,
            ProductId = dto.ProductId,
            UnitPrice = dto.UnitPrice,
            UnitsInStock = dto.UnitsInStock,
            DateAdded = dto.DateAdded,
            LastUpdated = dto.LastUpdated,
            ModifiedBy = dto.ModifiedBy
        };
    }
}
