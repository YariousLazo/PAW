using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class SupplierDTO
{
    [JsonPropertyName("supplierId")]
    public int SupplierId { get; set; }
    [JsonPropertyName("supplierName")]
    [StringLength(255)]
    public string? SupplierName { get; set; }
    [JsonPropertyName("contactName")]
    [StringLength(255)]
    public string? ContactName { get; set; }
    [JsonPropertyName("contactTitle")]
    [StringLength(255)]
    public string? ContactTitle { get; set; }
    [JsonPropertyName("phone")]
    [StringLength(255)]
    public string? Phone { get; set; }
    [JsonPropertyName("address")]
    [StringLength(1000)]
    public string? Address { get; set; }
    [JsonPropertyName("city")]
    [StringLength(255)]
    public string? City { get; set; }
    [JsonPropertyName("country")]
    [StringLength(255)]
    public string? Country { get; set; }
    [JsonPropertyName("modifiedBy")]
    [StringLength(255)]
    public string? ModifiedBy { get; set; }
    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    public static SupplierDTO ConvertFrom(Supplier supplier)
    {
        return new SupplierDTO
        {
            SupplierId = supplier.SupplierId,
            SupplierName = supplier.SupplierName,
            ContactName = supplier.ContactName,
            ContactTitle = supplier.ContactTitle,
            Phone = supplier.Phone,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            ModifiedBy = supplier.ModifiedBy,
            LastModified = supplier.LastModified
        };
    }

    public static Supplier ConvertTo(SupplierDTO dto)
    {
        return new Supplier
        {
            SupplierId = dto.SupplierId,
            SupplierName = dto.SupplierName,
            ContactName = dto.ContactName,
            ContactTitle = dto.ContactTitle,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            ModifiedBy = dto.ModifiedBy,
            LastModified = dto.LastModified
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(SupplierDTO dto, Supplier entity)
    {
        entity.SupplierName = dto.SupplierName;
        entity.ContactName = dto.ContactName;
        entity.ContactTitle = dto.ContactTitle;
        entity.Phone = dto.Phone;
        entity.Address = dto.Address;
        entity.City = dto.City;
        entity.Country = dto.Country;
        entity.ModifiedBy = dto.ModifiedBy;
    }
}
