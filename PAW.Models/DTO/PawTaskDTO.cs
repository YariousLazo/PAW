using System.Text.Json.Serialization;
using PawTask = PAW.Models.Task;

namespace PAW.Models.DTO;

public class PawTaskDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("modifiedBy")]
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
}
