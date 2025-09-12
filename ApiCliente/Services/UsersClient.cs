namespace ApiCliente.Services
{
    public class UsersClient(HttpClient http)
    {
        private const string BasePath = "api/usuarios";

        public async Task<IReadOnlyList<string>> ListarNombresAsync(CancellationToken ct = default)
        {
            var usuarios = await http.GetFromJsonAsync<List<UsuarioDto>>(BasePath, ct) ?? [];
            return usuarios.Select(u => u.Nombre).ToList();
        }

        public async Task<bool> ExistePorNombreAsync(string nombre, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            var response = await http.GetAsync($"{BasePath}/exists?nombre={Uri.EscapeDataString(nombre)}", ct);
            response.EnsureSuccessStatusCode();
            var exists = await response.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
            return exists;
        }

        // Nuevo: validar existencia por Id
        public async Task<bool> ExistePorIdAsync(int id, CancellationToken ct = default)
        {
            var resp = await http.GetAsync($"{BasePath}/{id}", ct);
            return resp.IsSuccessStatusCode; // 200 = existe, 404 = no existe
        }

        public record UsuarioDto(int Id, string Nombre);
    }
}
