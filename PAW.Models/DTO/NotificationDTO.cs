using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class NotificationDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("message")]
    [StringLength(255)]
    public string Message { get; set; } = null!;
    [JsonPropertyName("isRead")]
    public bool? IsRead { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    public static NotificationDTO ConvertFrom(Notification notification)
    {
        return new NotificationDTO
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }

    public static Notification ConvertTo(NotificationDTO dto)
    {
        return new Notification
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Message = dto.Message,
            IsRead = dto.IsRead,
            CreatedAt = dto.CreatedAt
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(NotificationDTO dto, Notification entity)
    {
        entity.UserId = dto.UserId;
        entity.Message = dto.Message;
        entity.IsRead = dto.IsRead;
    }
}
