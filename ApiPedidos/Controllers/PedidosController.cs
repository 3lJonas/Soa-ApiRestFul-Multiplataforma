using ApiPedidos.Data;
using ApiPedidos.Dtos;
using ApiPedidos.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ApiPedidos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController(AppDbContext db) : ControllerBase
    {
        // POST: api/pedidos
        [HttpPost]
        public async Task<ActionResult<PedidoDto>> CrearPedido([FromBody] PedidoCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Producto)) return BadRequest("El producto es requerido.");
            if (dto.Cantidad <= 0) return BadRequest("La cantidad debe ser mayor a cero.");

            Usuario? usuario = null;
            if (dto.UsuarioId is int uid)
            {
                usuario = await db.Usuarios.FindAsync(uid);
                if (usuario is null) return BadRequest($"UsuarioId '{uid}' no existe.");
            }
            else if (!string.IsNullOrWhiteSpace(dto.UsuarioNombre))
            {
                usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Nombre == dto.UsuarioNombre);
                if (usuario is null) return BadRequest($"El usuario '{dto.UsuarioNombre}' no existe.");
            }
            else
            {
                return BadRequest("Debe especificar UsuarioId o UsuarioNombre.");
            }

            var pedido = new Pedido
            {
                Producto = dto.Producto,
                Cantidad = dto.Cantidad,
                UsuarioId = usuario.Id
            };

            db.Pedidos.Add(pedido);
            await db.SaveChangesAsync();

            var result = new PedidoDto(pedido.Id, pedido.Producto, pedido.Cantidad, pedido.FechaCreacion, usuario.Id, usuario.Nombre);
            return CreatedAtAction(nameof(ObtenerPedido), new { id = pedido.Id }, result);
        }

        // GET: api/pedidos?usuarioId=123
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> ListarPedidos([FromQuery] int? usuarioId)
        {
            var query = db.Pedidos
                .AsNoTracking()
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaCreacion)
                .AsQueryable();

            if (usuarioId.HasValue)
            {
                query = query.Where(p => p.UsuarioId == usuarioId.Value);
            }

            var pedidos = await query
                .Select(p => new PedidoDto(p.Id, p.Producto, p.Cantidad, p.FechaCreacion, p.UsuarioId, p.Usuario!.Nombre))
                .ToListAsync();

            return Ok(pedidos);
        }

        // GET: api/pedidos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PedidoDto>> ObtenerPedido(int id)
        {
            var p = await db.Pedidos.AsNoTracking().Include(x => x.Usuario).FirstOrDefaultAsync(x => x.Id == id);
            if (p is null) return NotFound();
            return Ok(new PedidoDto(p.Id, p.Producto, p.Cantidad, p.FechaCreacion, p.UsuarioId, p.Usuario!.Nombre));
        }
    }
}
