using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComponentController(ILogger<ComponentController> logger, IComponentRepository componentRepository) : ControllerBase
    {
        [HttpGet(Name = "GetComponents")]
        public async Task<IEnumerable<ComponentDTO>> GetAll()
        {
            var items = await componentRepository.ReadAsync() ?? [];
            return items.Select(ComponentDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetComponentById")]
        public async Task<ActionResult<ComponentDTO>> GetById(int id)
        {
            // Id is numeric(18,0) (decimal), so DbSet.Find(int) cannot be used here.
            var item = (await componentRepository.ReadAsync()).FirstOrDefault(x => x.Id == id);
            if (item is null) return NotFound();
            return ComponentDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] ComponentDTO dto)
        {
            var entity = ComponentDTO.ConvertTo(dto);
            return await componentRepository.UpsertAsync(entity, dto.Id > 0);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = (await componentRepository.ReadAsync()).FirstOrDefault(x => x.Id == id);
            if (item is null) return NotFound();
            return await componentRepository.DeleteAsync(item);
        }
    }
}
