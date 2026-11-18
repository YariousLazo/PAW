using PAW.Models.DTO;

namespace PAW.Web.Models;

/// <summary>
/// Product plus the Category, Inventory and Supplier associated to it.
/// </summary>
public class ProductDetailsViewModel
{
    public ProductDTO Product { get; set; } = new();
    public CategoryDTO? Category { get; set; }
    public InventoryDTO? Inventory { get; set; }
    public SupplierDTO? Supplier { get; set; }
}
