using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController(ILogger<SupplierController> logger, ISupplierRepository supplierRepository) : ControllerBase
    {
        [HttpGet(Name = "GetSuppliers")]
        public async Task<IEnumerable<SupplierDTO>> GetAll()
        {
            var items = await supplierRepository.ReadAsync() ?? [];
            return items.Select(SupplierDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetSupplierById")]
        public async Task<ActionResult<SupplierDTO>> GetById(int id)
        {
            var item = await supplierRepository.FindAsync(id);
            if (item is null) return NotFound();
            return SupplierDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] SupplierDTO dto)
        {
            if (dto.SupplierId > 0)
            {
                var existing = await supplierRepository.FindAsync(dto.SupplierId);
                if (existing is null) return NotFound();
                SupplierDTO.CopyTo(dto, existing);
                existing.LastModified = DateTime.Now;
                return await supplierRepository.UpdateAsync(existing);
            }

            var entity = SupplierDTO.ConvertTo(dto);
            entity.LastModified = DateTime.Now;
            return await supplierRepository.CreateAsync(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = await supplierRepository.FindAsync(id);
            if (item is null) return NotFound();
            return await supplierRepository.DeleteAsync(item);
        }
    }
}
