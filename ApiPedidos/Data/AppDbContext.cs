
using ApiPedidos.models;
using Microsoft.EntityFrameworkCore;



namespace ApiPedidos.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            modelBuilder.Entity<Pedido>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Producto).IsRequired().HasMaxLength(200);
                e.Property(x => x.Cantidad).IsRequired();
                e.Property(x => x.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(p => p.Usuario)
                 .WithMany(u => u.Pedidos)
                 .HasForeignKey(p => p.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}