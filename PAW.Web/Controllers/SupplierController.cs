using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _service;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService service, ILogger<SupplierController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var result = await _service.GetSuppliersAsync();
            return View(PagedList<SupplierDTO>.Create(result, page));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : PartialView("_Details", dto);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form", new SupplierDTO());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null) return NotFound();
            return PartialView("_Form", dto);
        }

        // Creates or updates (the API decides). On validation errors the form partial is returned again.
        [HttpPost]
        public async Task<IActionResult> Save(SupplierDTO dto)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", dto);

            try
            {
                if (!await _service.SaveAsync(dto))
                {
                    ModelState.AddModelError(string.Empty, "The supplier could not be saved.");
                    return PartialView("_Form", dto);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save the supplier");
                ModelState.AddModelError(string.Empty, "The supplier could not be saved. Make sure PAW.API is running.");
                return PartialView("_Form", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null
                ? NotFound()
                : PartialView("_DeleteConfirm", new DeleteConfirmModel("Supplier", id, dto.SupplierName));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (await _service.DeleteAsync(id))
                    return Json(new { success = true });

                ModelState.AddModelError(string.Empty, "The supplier could not be deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete the supplier {Id}", id);
                ModelState.AddModelError(string.Empty, "The supplier could not be deleted. It may be in use or the API is not running.");
            }

            return PartialView("_DeleteConfirm", new DeleteConfirmModel("Supplier", id, "supplier #" + id));
        }
    }
}
