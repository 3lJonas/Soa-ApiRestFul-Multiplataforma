using ApiCliente.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCliente.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController(UsersClient users) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var nombres = await users.ListarNombresAsync(ct);
            return Ok(nombres);
        }
    }
}
