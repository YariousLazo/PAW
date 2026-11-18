using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IInventoryService inventoryService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var result = await _productService.GetProductsAsync();
            return View(PagedList<ProductDTO>.Create(result, page));
        }

        // Product plus its Category, Inventory and Supplier (each one reuses its own _DetailsBody partial).
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();

            var viewModel = new ProductDetailsViewModel { Product = product };
            if (product.CategoryId is int categoryId)
                viewModel.Category = await _categoryService.GetByIdAsync(categoryId);
            if (product.InventoryId is int inventoryId)
                viewModel.Inventory = await _inventoryService.GetByIdAsync(inventoryId);
            if (product.SupplierId is int supplierId)
                viewModel.Supplier = await _supplierService.GetByIdAsync(supplierId);

            return PartialView("_Details", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadListsAsync();
            return PartialView("_Form", new ProductDTO());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _productService.GetByIdAsync(id);
            if (dto is null) return NotFound();

            await LoadListsAsync();
            return PartialView("_Form", dto);
        }

        // Creates or updates (the API decides). On validation errors the form partial is returned again.
        [HttpPost]
        public async Task<IActionResult> Save(ProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return PartialView("_Form", dto);
            }

            try
            {
                if (await _productService.SaveAsync(dto))
                    return Json(new { success = true });

                ModelState.AddModelError(string.Empty, "The product could not be saved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save the product");
                ModelState.AddModelError(string.Empty, "The product could not be saved. Make sure PAW.API is running.");
            }

            await LoadListsAsync();
            return PartialView("_Form", dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _productService.GetByIdAsync(id);
            return dto is null
                ? NotFound()
                : PartialView("_DeleteConfirm", new DeleteConfirmModel("Product", id, dto.Name));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (await _productService.DeleteAsync(id))
                    return Json(new { success = true });

                ModelState.AddModelError(string.Empty, "The product could not be deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete the product {Id}", id);
                ModelState.AddModelError(string.Empty, "The product could not be deleted. It may be in use or the API is not running.");
            }

            return PartialView("_DeleteConfirm", new DeleteConfirmModel("Product", id, "product #" + id));
        }

        // Lists used by the dropdowns of Views/Product/_Form.cshtml
        private async Task LoadListsAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            var suppliers = await _supplierService.GetSuppliersAsync();
            var inventories = await _inventoryService.GetInventoriesAsync();

            ViewBag.Categories = categories
                .Select(c => new SelectListItem(c.CategoryName ?? $"Category #{c.CategoryId}", c.CategoryId.ToString()))
                .ToList();
            ViewBag.Suppliers = suppliers
                .Select(s => new SelectListItem(s.SupplierName ?? $"Supplier #{s.SupplierId}", s.SupplierId.ToString()))
                .ToList();
            ViewBag.Inventories = inventories
                .Select(i => new SelectListItem($"Inventory #{i.InventoryId}", i.InventoryId.ToString()))
                .ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
