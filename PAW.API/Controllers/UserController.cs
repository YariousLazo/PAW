using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ILogger<UserController> logger, IUserRepository userRepository) : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var items = await userRepository.ReadAsync() ?? [];
            return items.Select(UserDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var item = await userRepository.FindAsync(id);
            if (item is null) return NotFound();
            return UserDTO.ConvertFrom(item);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Save([FromBody] UserDTO dto)
        {
            if (dto.UserId > 0)
            {
                // Update the tracked entity so the stored PasswordHash is not overwritten.
                var existing = await userRepository.FindAsync(dto.UserId);
                if (existing is null) return NotFound();
                UserDTO.CopyTo(dto, existing);
                return await userRepository.UpdateAsync(existing);
            }

            return await userRepository.CreateAsync(UserDTO.ConvertTo(dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var item = await userRepository.FindAsync(id);
            if (item is null) return NotFound();
            return await userRepository.DeleteAsync(item);
        }
    }
}
