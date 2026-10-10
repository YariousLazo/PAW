using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class UserRoleDTO
{
    [JsonPropertyName("id")]
    public decimal Id { get; set; }
    [JsonPropertyName("roleId")]
    public decimal? RoleId { get; set; }
    [JsonPropertyName("userId")]
    public decimal? UserId { get; set; }

    public static UserRoleDTO ConvertFrom(UserRole userRole)
    {
        return new UserRoleDTO
        {
            Id = userRole.Id,
            RoleId = userRole.RoldId,
            UserId = userRole.UserId
        };
    }

    public static UserRole ConvertTo(UserRoleDTO dto)
    {
        return new UserRole
        {
            Id = dto.Id,
            RoldId = dto.RoleId,
            UserId = dto.UserId
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(UserRoleDTO dto, UserRole entity)
    {
        entity.RoldId = dto.RoleId;
        entity.UserId = dto.UserId;
    }
}
