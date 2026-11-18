using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetRolesAsync();
    Task<RoleDTO?> GetByIdAsync(int id);
    Task<bool> SaveAsync(RoleDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class RoleService : ServiceBase, IRoleService
{
    private const string _path = "Role";
    private readonly IRestProvider _restProvider;

    public RoleService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var items = await JsonProvider.DeserializeAsync<IEnumerable<RoleDTO>>(response);
        return items;
    }

    public async Task<RoleDTO?> GetByIdAsync(int id)
    {
        try
        {
            // The trailing slash keeps "Role" in the URL when the id is appended.
            var response = await _restProvider.GetAsync(SetPathUrl(_path) + "/", id.ToString());
            return await JsonProvider.DeserializeAsync<RoleDTO>(response);
        }
        catch (Exception ex) when (IsNotFound(ex))
        {
            return null;
        }
    }

    public async Task<bool> SaveAsync(RoleDTO dto)
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
