using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class ComponentDTO
{
    [JsonPropertyName("id")]
    public decimal Id { get; set; }
    [JsonPropertyName("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    [JsonPropertyName("content")]
    [StringLength(100)]
    public string Content { get; set; } = null!;

    public static ComponentDTO ConvertFrom(Component component)
    {
        return new ComponentDTO
        {
            Id = component.Id,
            Name = component.Name,
            Content = component.Content
        };
    }

    public static Component ConvertTo(ComponentDTO dto)
    {
        return new Component
        {
            Id = dto.Id,
            Name = dto.Name,
            Content = dto.Content
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(ComponentDTO dto, Component entity)
    {
        entity.Name = dto.Name;
        entity.Content = dto.Content;
    }
}
