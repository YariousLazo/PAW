using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class UserDTO
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }
    [JsonPropertyName("roleId")]
    public int? RoleId { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("modifiedBy")]
    public string? ModifiedBy { get; set; }
    [JsonPropertyName("lastModifiedBy")]
    public string? LastModifiedBy { get; set; }
    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    public static UserDTO ConvertFrom(User user)
    {
        return new UserDTO
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            RoleId = user.RoleId,
            CreatedAt = user.CreatedAt,
            ModifiedBy = user.ModifiedBy,
            LastModifiedBy = user.LastModifiedBy,
            LastModified = user.LastModified
        };
    }

    public static User ConvertTo(UserDTO dto)
    {
        return new User
        {
            UserId = dto.UserId,
            Username = dto.Username,
            Email = dto.Email,
            IsActive = dto.IsActive,
            RoleId = dto.RoleId,
            CreatedAt = dto.CreatedAt,
            ModifiedBy = dto.ModifiedBy,
            LastModifiedBy = dto.LastModifiedBy,
            LastModified = dto.LastModified
        };
    }

    /// <summary>
    /// Copies the DTO values onto an existing entity. PasswordHash is never exposed by the DTO, so it is left untouched.
    /// </summary>
    public static void CopyTo(UserDTO dto, User entity)
    {
        entity.Username = dto.Username;
        entity.Email = dto.Email;
        entity.IsActive = dto.IsActive;
        entity.RoleId = dto.RoleId;
        entity.CreatedAt = dto.CreatedAt;
        entity.ModifiedBy = dto.ModifiedBy;
        entity.LastModifiedBy = dto.LastModifiedBy;
        entity.LastModified = dto.LastModified;
    }
}
