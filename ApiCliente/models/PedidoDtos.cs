namespace ApiCliente.models
{
    public record PedidoCreateDto(string UsuarioNombre, string Producto, int Cantidad);

    public record PedidoDto(int Id, string Producto, int Cantidad, DateTime FechaCreacion, int UsuarioId, string UsuarioNombre);
}
