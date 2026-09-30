using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController(ILogger<UserRoleController> logger, IUserRoleRepository userRoleRepository) : ControllerBase
    {
        [HttpGet(Name = "GetUserRoles")]
        public async Task<IEnumerable<UserRoleDTO>> GetAll()
        {
            var items = await userRoleRepository.ReadAsync() ?? [];
            return items.Select(UserRoleDTO.ConvertFrom);
        }

        // UserRole is a keyless entity (HasNoKey in the DbContext): EF Core can read it,
        // but it cannot track it, so there are no GetById / Save / Delete endpoints.
    }
}
