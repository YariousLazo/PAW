using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface ITaskApiService
{
    Task<IEnumerable<PawTaskDTO>> GetAllAsync();
    Task<PawTaskDTO?> GetByIdAsync(int id);
    Task CreateAsync(PawTaskDTO dto);
    Task UpdateAsync(PawTaskDTO dto);
    Task DeleteAsync(int id);
}

/// <summary>
/// Consumes PAW.MinimalAPI (/tasks). It does not use PAW.API: the Minimal API has its own address,
/// configured in appsettings.json ("MinimalApi:BaseUrl").
/// </summary>
public class TaskApiService : ServiceBase, ITaskApiService
{
    private const string _path = "tasks";
    private readonly IRestProvider _restProvider;

    public TaskApiService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;

        var baseUrl = configuration["MinimalApi:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            BaseUrl = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
        }
    }

    public async Task<IEnumerable<PawTaskDTO>> GetAllAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        return await JsonProvider.DeserializeAsync<IEnumerable<PawTaskDTO>>(response);
    }

    public async Task<PawTaskDTO?> GetByIdAsync(int id)
    {
        try
        {
            // The trailing slash keeps "tasks" in the URL when the id is appended.
            var response = await _restProvider.GetAsync(SetPathUrl(_path) + "/", id.ToString());
            return await JsonProvider.DeserializeAsync<PawTaskDTO>(response);
        }
        catch (Exception ex) when (IsNotFound(ex))
        {
            return null;
        }
    }

    public async Task CreateAsync(PawTaskDTO dto)
    {
        await _restProvider.PostAsync(SetPathUrl(_path), JsonProvider.Serialize(dto));
    }

    public async Task UpdateAsync(PawTaskDTO dto)
    {
        await _restProvider.PutAsync(SetPathUrl(_path) + "/", dto.Id.ToString(), JsonProvider.Serialize(dto));
    }

    public async Task DeleteAsync(int id)
    {
        await _restProvider.DeleteAsync(SetPathUrl(_path) + "/", id.ToString());
    }
}
