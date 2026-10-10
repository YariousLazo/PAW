using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository) : ControllerBase
    {
        [HttpGet(Name = "GetCategories")]
        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            var items = await categoryRepository.ReadAsync() ?? [];
            return items.Select(CategoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var item = await categoryRepository.FindAsync(id);
            if (item is null) return NotFound();
            return CategoryDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] CategoryDTO dto)
        {
            if (dto.CategoryId > 0)
            {
                var existing = await categoryRepository.FindAsync(dto.CategoryId);
                if (existing is null) return NotFound();
                CategoryDTO.CopyTo(dto, existing);
                existing.LastModified = DateTime.Now;
                return await categoryRepository.UpdateAsync(existing);
            }

            var entity = CategoryDTO.ConvertTo(dto);
            entity.LastModified = DateTime.Now;
            return await categoryRepository.CreateAsync(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = await categoryRepository.FindAsync(id);
            if (item is null) return NotFound();
            return await categoryRepository.DeleteAsync(item);
        }
    }
}
