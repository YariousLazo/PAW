using PawTask = PAW.Models.Task;

namespace PAW.MinimalAPI.Data;

/// <summary>
/// In-memory implementation of <see cref="ITaskRepository"/>.
/// Registered as a singleton so the data lives as long as the API is running.
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    public const string Pending = "Pending";
    public const string InProgress = "In Progress";
    public const string Completed = "Completed";

    private readonly object _lock = new();
    private readonly List<PawTask> _tasks = [];
    private int _nextId = 1;

    public InMemoryTaskRepository()
    {
        var now = DateTime.Now;
        Seed("Design the database", "Define tables and relationships for the PAW project.", Completed, now.AddDays(-5));
        Seed("Create the Product screens", "Index, Details, Edit, Add and Delete using partial views.", InProgress, now.AddDays(2));
        Seed("Build the Minimal API", "Expose the Tasks with a Minimal API using Dependency Injection.", InProgress, now.AddDays(4));
        Seed("Cache the exchange rate", "Load the exchange rate from JavaScript and cache it.", Completed, now.AddDays(-1));
        Seed("Write the project documentation", "Explain how to run the API and the web app.", Pending, now.AddDays(10));
    }

    public Task<IEnumerable<PawTask>> GetAllAsync()
    {
        lock (_lock)
        {
            IEnumerable<PawTask> result = _tasks.OrderBy(t => t.Id).Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<PawTask?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return Task.FromResult<PawTask?>(task is null ? null : Clone(task));
        }
    }

    public Task<PawTask> CreateAsync(PawTask task)
    {
        lock (_lock)
        {
            var now = DateTime.Now;
            var created = Clone(task);
            created.Id = _nextId++;
            created.CreatedAt = now;
            created.LastModified = now;
            created.Status = string.IsNullOrWhiteSpace(created.Status) ? Pending : created.Status;
            created.ModifiedBy = string.IsNullOrWhiteSpace(created.ModifiedBy) ? "api" : created.ModifiedBy;
            _tasks.Add(created);
            return Task.FromResult(Clone(created));
        }
    }

    public Task<bool> UpdateAsync(int id, PawTask input)
    {
        lock (_lock)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == id);
            if (existing is null) return Task.FromResult(false);

            existing.Name = input.Name;
            existing.Description = input.Description;
            existing.Status = string.IsNullOrWhiteSpace(input.Status) ? existing.Status : input.Status;
            existing.DueDate = input.DueDate;
            existing.ModifiedBy = string.IsNullOrWhiteSpace(input.ModifiedBy) ? "api" : input.ModifiedBy;
            existing.LastModified = DateTime.Now; // CreatedAt is kept as it was
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == id);
            if (existing is null) return Task.FromResult(false);
            _tasks.Remove(existing);
            return Task.FromResult(true);
        }
    }

    private void Seed(string name, string description, string status, DateTime dueDate)
    {
        var now = DateTime.Now;
        _tasks.Add(new PawTask
        {
            Id = _nextId++,
            Name = name,
            Description = description,
            Status = status,
            DueDate = dueDate,
            CreatedAt = now,
            LastModified = now,
            ModifiedBy = "seed"
        });
    }

    private static PawTask Clone(PawTask t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Description = t.Description,
        Status = t.Status,
        DueDate = t.DueDate,
        CreatedAt = t.CreatedAt,
        LastModified = t.LastModified,
        ModifiedBy = t.ModifiedBy
    };
}
