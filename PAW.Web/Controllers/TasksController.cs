using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    /// <summary>
    /// Tasks screen backed by PAW.MinimalAPI (not by PAW.API / the database).
    /// Same modal flow as the other entities: Details / Create / Edit / Delete are partial views.
    /// </summary>
    public class TasksController : Controller
    {
        private readonly ITaskApiService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskApiService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            IEnumerable<PawTaskDTO> items = [];
            try
            {
                items = await _taskService.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not load the tasks from PAW.MinimalAPI");
                ViewData["ErrorMessage"] = "The tasks could not be loaded. Make sure PAW.MinimalAPI is running.";
            }

            return View(PagedList<PawTaskDTO>.Create(items, page));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _taskService.GetByIdAsync(id);
            return dto is null ? NotFound() : PartialView("_Details", dto);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form", new PawTaskDTO { Status = "Pending", DueDate = DateTime.Today.AddDays(7) });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _taskService.GetByIdAsync(id);
            return dto is null ? NotFound() : PartialView("_Form", dto);
        }

        // Creates (Id = 0) or updates (Id > 0). On validation errors the form partial is returned again.
        [HttpPost]
        public async Task<IActionResult> Save(PawTaskDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                ModelState.AddModelError(nameof(dto.Name), "Name is required.");

            if (!ModelState.IsValid)
                return PartialView("_Form", dto);

            try
            {
                if (dto.Id > 0)
                    await _taskService.UpdateAsync(dto);
                else
                    await _taskService.CreateAsync(dto);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save the task {Id}", dto.Id);
                ModelState.AddModelError(string.Empty, "The task could not be saved. Make sure PAW.MinimalAPI is running.");
                return PartialView("_Form", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _taskService.GetByIdAsync(id);
            return dto is null
                ? NotFound()
                : PartialView("_DeleteConfirm", new DeleteConfirmModel("Task", dto.Id, dto.Name));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, bool confirmed = true)
        {
            try
            {
                await _taskService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete the task {Id}", id);
                ModelState.AddModelError(string.Empty, "The task could not be deleted.");
                return PartialView("_DeleteConfirm", new DeleteConfirmModel("Task", id, $"task #{id}"));
            }
        }
    }
}
