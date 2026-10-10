using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class RoleDTO
{
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }
    [JsonPropertyName("roleName")]
    [StringLength(255)]
    public string? RoleName { get; set; }

    public static RoleDTO ConvertFrom(Role role)
    {
        return new RoleDTO
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName
        };
    }

    public static Role ConvertTo(RoleDTO dto)
    {
        return new Role
        {
            RoleId = dto.RoleId,
            RoleName = dto.RoleName
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(RoleDTO dto, Role entity)
    {
        entity.RoleName = dto.RoleName;
    }
}
