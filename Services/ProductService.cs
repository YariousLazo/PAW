using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProductsAsync();
    Task<ProductDTO?> GetByIdAsync(int id);
    Task<bool> SaveAsync(ProductDTO dto);
    Task<bool> DeleteAsync(int id);
}

public class ProductService : ServiceBase, IProductService
{
    private const string _path = "Product";
    private readonly IRestProvider _restProvider;

    public ProductService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var products = await JsonProvider.DeserializeAsync<IEnumerable<ProductDTO>>(response);
        return products;
    }

    public async Task<ProductDTO?> GetByIdAsync(int id)
    {
        try
        {
            // The trailing slash keeps "Product" in the URL when the id is appended.
            var response = await _restProvider.GetAsync(SetPathUrl(_path) + "/", id.ToString());
            return await JsonProvider.DeserializeAsync<ProductDTO>(response);
        }
        catch (Exception ex) when (IsNotFound(ex))
        {
            return null;
        }
    }

    public async Task<bool> SaveAsync(ProductDTO dto)
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
