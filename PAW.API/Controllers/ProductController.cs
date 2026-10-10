using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(ILogger<ProductController> logger, IProductRepository productRepository) : ControllerBase
    {
        [HttpGet(Name = "GetProducts")]
        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var products = await productRepository.ReadAsync() ?? [];
            return products.Select(ProductDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await productRepository.FindAsync(id);
            if (product is null) return NotFound();
            return ProductDTO.ConvertFrom(product);
        }

        /*[HttpPost("filter", Name = "FilterProducts")]
        public async Task<IEnumerable<Product>> Filter(ConditionViewModel condition)
        {
            var predicate = ConditionResolver<Product>.ResolveCondition(condition.Criteria, condition.Property, condition.Value, condition.Start, condition.End);
            var results = await businessProduct.Filter(predicate);
            return results;
        }*/

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] ProductDTO dto)
        {
            if (dto.ProductId > 0)
            {
                var existing = await productRepository.FindAsync(dto.ProductId);
                if (existing is null) return NotFound();
                ProductDTO.CopyTo(dto, existing);
                existing.LastModified = DateTime.Now;
                return await productRepository.UpdateAsync(existing);
            }

            var entity = ProductDTO.ConvertTo(dto);
            entity.LastModified = DateTime.Now;
            return await productRepository.CreateAsync(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var product = await productRepository.FindAsync(id);
            if (product is null) return NotFound();
            return await productRepository.DeleteAsync(product);
        }
    }
}
