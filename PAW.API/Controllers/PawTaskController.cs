using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PawTaskController(ILogger<PawTaskController> logger, IPawTaskRepository pawTaskRepository) : ControllerBase
    {
        [HttpGet(Name = "GetPawTasks")]
        public async Task<IEnumerable<PawTaskDTO>> GetAll()
        {
            var items = await pawTaskRepository.ReadAsync() ?? [];
            return items.Select(PawTaskDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetPawTaskById")]
        public async Task<ActionResult<PawTaskDTO>> GetById(int id)
        {
            var item = await pawTaskRepository.FindAsync(id);
            if (item is null) return NotFound();
            return PawTaskDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] PawTaskDTO dto)
        {
            if (dto.Id > 0)
            {
                var existing = await pawTaskRepository.FindAsync(dto.Id);
                if (existing is null) return NotFound();
                PawTaskDTO.CopyTo(dto, existing);
                existing.LastModified = DateTime.Now;
                return await pawTaskRepository.UpdateAsync(existing);
            }

            var entity = PawTaskDTO.ConvertTo(dto);
            entity.CreatedAt = DateTime.Now;
            entity.LastModified = DateTime.Now;
            return await pawTaskRepository.CreateAsync(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = await pawTaskRepository.FindAsync(id);
            if (item is null) return NotFound();
            return await pawTaskRepository.DeleteAsync(item);
        }
    }
}
