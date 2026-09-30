using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActionController(ILogger<UserActionController> logger, IUserActionRepository userActionRepository) : ControllerBase
    {
        [HttpGet(Name = "GetUserActions")]
        public async Task<IEnumerable<UserActionDTO>> GetAll()
        {
            var items = await userActionRepository.ReadAsync() ?? [];
            return items.Select(UserActionDTO.ConvertFrom);
        }

        // UserAction is a keyless entity (HasNoKey in the DbContext): EF Core can read it,
        // but it cannot track it, so there are no GetById / Save / Delete endpoints.
    }
}
