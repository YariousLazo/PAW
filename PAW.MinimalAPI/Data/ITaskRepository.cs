using PawTask = PAW.Models.Task;

namespace PAW.MinimalAPI.Data;

/// <summary>
/// Abstraction used by the endpoints to get/save Tasks.
/// It is resolved through Dependency Injection (see Program.cs), so the endpoints
/// never know where the data really lives (no Entity Framework DbContext here).
/// </summary>
public interface ITaskRepository
{
    Task<IEnumerable<PawTask>> GetAllAsync();
    Task<PawTask?> GetByIdAsync(int id);
    Task<PawTask> CreateAsync(PawTask task);
    Task<bool> UpdateAsync(int id, PawTask input);
    Task<bool> DeleteAsync(int id);
}
