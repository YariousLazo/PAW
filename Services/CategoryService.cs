using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
    Task<CategoryDTO?> GetByIdAsync(int id);
    Task<bool> SaveAsync(CategoryDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class CategoryService : ServiceBase, ICategoryService
{
    private const string _path = "Category";
    private readonly IRestProvider _restProvider;

    public CategoryService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var items = await JsonProvider.DeserializeAsync<IEnumerable<CategoryDTO>>(response);
        return items;
    }

    public async Task<CategoryDTO?> GetByIdAsync(int id)
    {
        try
        {
            // The trailing slash keeps "Category" in the URL when the id is appended.
            var response = await _restProvider.GetAsync(SetPathUrl(_path) + "/", id.ToString());
            return await JsonProvider.DeserializeAsync<CategoryDTO>(response);
        }
        catch (Exception ex) when (IsNotFound(ex))
        {
            return null;
        }
    }

    public async Task<bool> SaveAsync(CategoryDTO dto)
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
