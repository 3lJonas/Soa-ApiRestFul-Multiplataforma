using ApiCliente.models;
using ApiCliente.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiCliente.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController(PedidosClient pedidosClient, UsersClient usersClient) : ControllerBase
    {
        [HttpPost("pedidos")]
        public async Task<IActionResult> CrearPedido([FromBody] PedidoCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.UsuarioNombre))
                return BadRequest("El UsuarioNombre es requerido.");
            if (!await usersClient.ExistePorNombreAsync(dto.UsuarioNombre, ct))
                return BadRequest($"El usuario '{dto.UsuarioNombre}' no existe.");
            if (string.IsNullOrWhiteSpace(dto.Producto))
                return BadRequest("El producto es requerido.");
            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a cero.");

            var created = await pedidosClient.CrearPedidoAsync(dto, ct);
            return CreatedAtAction(nameof(ObtenerPedido), new { id = created.Id }, created);
        }

        [HttpGet("pedidos/{id:int}")]
        public async Task<IActionResult> ObtenerPedido([FromRoute] int id, CancellationToken ct)
        {
            var pedido = await pedidosClient.ObtenerPedidoAsync(id, ct);
            if (pedido is null) return NotFound();
            return Ok(pedido);
        }

        [HttpGet("pedidos")]
        public async Task<IActionResult> ListarPedidos(CancellationToken ct)
        {
            var pedidos = await pedidosClient.ListarPedidosAsync(ct);
            return Ok(pedidos);
        }

        // Nuevo: listar pedidos por Id de usuario
        [HttpGet("usuarios/{usuarioId:int}/pedidos")]
        public async Task<IActionResult> ListarPedidosPorUsuario([FromRoute] int usuarioId, CancellationToken ct)
        {
            if (!await usersClient.ExistePorIdAsync(usuarioId, ct))
                return NotFound($"UsuarioId '{usuarioId}' no existe.");

            var pedidos = await pedidosClient.ListarPedidosPorUsuarioAsync(usuarioId, ct);
            return Ok(pedidos);
        }
    }
}
