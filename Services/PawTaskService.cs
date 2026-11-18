using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IPawTaskService
{
    Task<IEnumerable<PawTaskDTO>> GetPawTasksAsync();
    Task<PawTaskDTO?> GetByIdAsync(int id);
    Task<bool> SaveAsync(PawTaskDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class PawTaskService : ServiceBase, IPawTaskService
{
    private const string _path = "PawTask";
    private readonly IRestProvider _restProvider;

    public PawTaskService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<PawTaskDTO>> GetPawTasksAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var items = await JsonProvider.DeserializeAsync<IEnumerable<PawTaskDTO>>(response);
        return items;
    }

    public async Task<PawTaskDTO?> GetByIdAsync(int id)
    {
        try
        {
            // The trailing slash keeps "PawTask" in the URL when the id is appended.
            var response = await _restProvider.GetAsync(SetPathUrl(_path) + "/", id.ToString());
            return await JsonProvider.DeserializeAsync<PawTaskDTO>(response);
        }
        catch (Exception ex) when (IsNotFound(ex))
        {
            return null;
        }
    }

    public async Task<bool> SaveAsync(PawTaskDTO dto)
    {
        var response = await _restProvider.PostAsync(SetPathUrl(_path), JsonProvider.Serialize(dto));
        return bool.TryParse(response, out var result) && result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(SetPathUrl(_path) + "/", id.ToString());
        return bool.TryParse(response, out var result) && result;
    }
}
