using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController(ILogger<InventoryController> logger, IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpGet(Name = "GetInventories")]
        public async Task<IEnumerable<InventoryDTO>> GetAll()
        {
            var items = await inventoryRepository.ReadAsync() ?? [];
            return items.Select(InventoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetInventoryById")]
        public async Task<ActionResult<InventoryDTO>> GetById(int id)
        {
            var item = await inventoryRepository.FindAsync(id);
            if (item is null) return NotFound();
            return InventoryDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] InventoryDTO dto)
        {
            var entity = InventoryDTO.ConvertTo(dto);
            return await inventoryRepository.UpsertAsync(entity, dto.InventoryId > 0);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = await inventoryRepository.FindAsync(id);
            if (item is null) return NotFound();
            return await inventoryRepository.DeleteAsync(item);
        }
    }
}
