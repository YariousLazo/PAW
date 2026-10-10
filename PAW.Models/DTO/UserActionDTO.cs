using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class UserActionDTO
{
    [JsonPropertyName("id")]
    public decimal Id { get; set; }
    [JsonPropertyName("name")]
    [StringLength(50)]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    [StringLength(100)]
    public string? Description { get; set; }

    public static UserActionDTO ConvertFrom(UserAction userAction)
    {
        return new UserActionDTO
        {
            Id = userAction.Id,
            Name = userAction.Name,
            Description = userAction.Description
        };
    }

    public static UserAction ConvertTo(UserActionDTO dto)
    {
        return new UserAction
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(UserActionDTO dto, UserAction entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}
