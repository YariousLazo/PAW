using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PawTask = PAW.Models.Task;

namespace PAW.Models.DTO;

public class PawTaskDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    [StringLength(255)]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    [StringLength(1000)]
    public string? Description { get; set; }
    [JsonPropertyName("status")]
    [StringLength(50)]
    public string? Status { get; set; }
    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("modifiedBy")]
    [StringLength(255)]
    public string? ModifiedBy { get; set; }
    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    public static PawTaskDTO ConvertFrom(PawTask pawTask)
    {
        return new PawTaskDTO
        {
            Id = pawTask.Id,
            Name = pawTask.Name,
            Description = pawTask.Description,
            Status = pawTask.Status,
            DueDate = pawTask.DueDate,
            CreatedAt = pawTask.CreatedAt,
            ModifiedBy = pawTask.ModifiedBy,
            LastModified = pawTask.LastModified
        };
    }

    public static PawTask ConvertTo(PawTaskDTO dto)
    {
        return new PawTask
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            DueDate = dto.DueDate,
            CreatedAt = dto.CreatedAt,
            ModifiedBy = dto.ModifiedBy,
            LastModified = dto.LastModified
        };
    }

    /// <summary>
    /// Copies only the editable values of the DTO onto an existing (tracked) entity.
    /// Fields that are not editable (audit dates, PasswordHash, ...) are left untouched.
    /// </summary>
    public static void CopyTo(PawTaskDTO dto, PawTask entity)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Status = dto.Status;
        entity.DueDate = dto.DueDate;
        entity.ModifiedBy = dto.ModifiedBy;
    }
}
