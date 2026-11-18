using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class ComponentController : Controller
    {
        private readonly IComponentService _service;
        private readonly ILogger<ComponentController> _logger;

        public ComponentController(IComponentService service, ILogger<ComponentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var result = await _service.GetComponentsAsync();
            return View(PagedList<ComponentDTO>.Create(result, page));
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
            return PartialView("_Form", new ComponentDTO());
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
        public async Task<IActionResult> Save(ComponentDTO dto)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", dto);

            try
            {
                if (!await _service.SaveAsync(dto))
                {
                    ModelState.AddModelError(string.Empty, "The component could not be saved.");
                    return PartialView("_Form", dto);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save the component");
                ModelState.AddModelError(string.Empty, "The component could not be saved. Make sure PAW.API is running.");
                return PartialView("_Form", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null
                ? NotFound()
                : PartialView("_DeleteConfirm", new DeleteConfirmModel("Component", id, dto.Name));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (await _service.DeleteAsync(id))
                    return Json(new { success = true });

                ModelState.AddModelError(string.Empty, "The component could not be deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete the component {Id}", id);
                ModelState.AddModelError(string.Empty, "The component could not be deleted. It may be in use or the API is not running.");
            }

            return PartialView("_DeleteConfirm", new DeleteConfirmModel("Component", id, "component #" + id));
        }
    }
}
