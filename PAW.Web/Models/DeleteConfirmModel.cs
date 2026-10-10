namespace PAW.Web.Models;

/// <summary>
/// Model for the reusable delete confirmation partial.
/// </summary>
public record DeleteConfirmModel(string Entity, int Id, string? Summary);
