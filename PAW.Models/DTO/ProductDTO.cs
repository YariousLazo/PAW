using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class ProductDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }
    [JsonPropertyName("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    [StringLength(1000)]
    public string? Description { get; set; }
    [JsonPropertyName("rating")]
    [Range(0, 5)]
    public int Rating { get; set; }
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }
    [JsonPropertyName("supplierId")]
    public int? SupplierId { get; set; }
    [JsonPropertyName("inventoryId")]
    public int? InventoryId { get; set; }
    [JsonPropertyName("modifiedBy")]
    [StringLength(255)]
    public string? ModifiedBy { get; set; }
    [JsonPropertyName("createdBy")]
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }
    [JsonPropertyName("modifiedDate")]
    public DateTime ModifiedDate { get; set; }

    public static ProductDTO ConvertFrom(Product product)
    {
        return new ProductDTO
        {
            Id = Guid.NewGuid(),
            ProductId = product.ProductId,
            Name = product.ProductName!,
            Description = product.Description,
            Rating = (int)(product.Rating ?? 0),
            CategoryId = product.CategoryId,
            SupplierId = product.SupplierId,
            InventoryId = product.InventoryId,
            ModifiedBy = product.ModifiedBy,
            CreatedBy = product.CreatedBy,
            Comments = string.Empty, // Assuming comments are not present in the Product entity
            CreatedDate = product.LastModified ?? DateTime.Now, // Assuming LastModified is used as CreatedDate
            ModifiedDate = product.LastModified ?? DateTime.Now // Assuming LastModified is used as ModifiedDate
        };
    }

    public static Product ConvertTo(ProductDTO productDTO)
    {
        return new Product
        {
            ProductId = productDTO.ProductId,
            ProductName = productDTO.Name,
            Description = productDTO.Description,
            Rating = productDTO.Rating,
            CategoryId = productDTO.CategoryId,
            SupplierId = productDTO.SupplierId,
            InventoryId = productDTO.InventoryId,
            ModifiedBy = productDTO.ModifiedBy,
            CreatedBy = productDTO.CreatedBy,
            LastModified = productDTO.ModifiedDate
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// </summary>
    public static void CopyTo(ProductDTO dto, Product entity)
    {
        entity.ProductName = dto.Name;
        entity.Description = dto.Description;
        entity.Rating = dto.Rating;
        entity.CategoryId = dto.CategoryId;
        entity.SupplierId = dto.SupplierId;
        entity.InventoryId = dto.InventoryId;
        entity.ModifiedBy = dto.ModifiedBy;
    }
}
