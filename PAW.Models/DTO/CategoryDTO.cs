using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class CategoryDTO
{
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }
    [JsonPropertyName("categoryName")]
    [StringLength(255)]
    public string? CategoryName { get; set; }
    [JsonPropertyName("description")]
    [StringLength(1000)]
    public string? Description { get; set; }
    [JsonPropertyName("modifiedBy")]
    [StringLength(255)]
    public string? ModifiedBy { get; set; }
    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    public static CategoryDTO ConvertFrom(Category category)
    {
        return new CategoryDTO
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            ModifiedBy = category.ModifiedBy,
            LastModified = category.LastModified
        };
    }

    public static Category ConvertTo(CategoryDTO dto)
    {
        return new Category
        {
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            ModifiedBy = dto.ModifiedBy,
            LastModified = dto.LastModified
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(CategoryDTO dto, Category entity)
    {
        entity.CategoryName = dto.CategoryName;
        entity.Description = dto.Description;
        entity.ModifiedBy = dto.ModifiedBy;
    }
}
