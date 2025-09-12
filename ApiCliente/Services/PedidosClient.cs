using ApiCliente.models;

namespace ApiCliente.Services
{
    public class PedidosClient(HttpClient http)
    {
        private const string BasePath = "api/pedidos";

        public async Task<PedidoDto> CrearPedidoAsync(PedidoCreateDto dto, CancellationToken ct = default)
        {
            var response = await http.PostAsJsonAsync(BasePath, dto, ct);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<PedidoDto>(cancellationToken: ct);
            if (created is null) throw new InvalidOperationException("No se pudo deserializar el pedido creado.");
            return created;
        }

        public async Task<PedidoDto?> ObtenerPedidoAsync(int id, CancellationToken ct = default)
        {
            var response = await http.GetAsync($"{BasePath}/{id}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PedidoDto>(cancellationToken: ct); // Fix: Corrected the parameter name from 'cancellationoken' to 'cancellationToken'
        }

        public async Task<IReadOnlyList<PedidoDto>> ListarPedidosAsync(CancellationToken ct = default)
        {
            var response = await http.GetAsync(BasePath, ct);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<PedidoDto>>(cancellationToken: ct);
            return list ?? [];
        }

        public async Task<IReadOnlyList<PedidoDto>> ListarPedidosPorUsuarioAsync(int usuarioId, CancellationToken ct = default)
        {
            var response = await http.GetAsync($"{BasePath}?usuarioId={usuarioId}", ct);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<PedidoDto>>(cancellationToken: ct);
            return list ?? [];
        }
    }
}
