namespace ApiPedidos.Dtos
{
    public record PedidoCreateDto(
     string Producto,
     int Cantidad,
     int? UsuarioId,
     string? UsuarioNombre
 );

    public record PedidoDto(
        int Id,
        string Producto,
        int Cantidad,
        DateTime FechaCreacion,
        int UsuarioId,
        string UsuarioNombre
    );
}
