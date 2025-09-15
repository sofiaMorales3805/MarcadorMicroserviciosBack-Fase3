using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Data;

public class MarcadorDbContext : DbContext
{
    public MarcadorDbContext(DbContextOptions<MarcadorDbContext> options)
            : base(options) { }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<MarcadorGlobal> Marcadores { get; set; }
        public DbSet<Falta> Faltas { get; set; }
        public DbSet<PartidoHistorico> PartidosHistoricos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Aquí configuramos las relaciones para evitar múltiples paths de cascada
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipo>().Property(e => e.Id).UseIdentityColumn(); // auto‑incrementa
            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoLocal)
                .WithMany()
                .HasForeignKey("EquipoLocalId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoVisitante)
                .WithMany()
                .HasForeignKey("EquipoVisitanteId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidoHistorico>()
                .Property(p => p.Id)
                .UseIdentityColumn();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Restrict);

            //Indice de username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //Indices sobre FK
            modelBuilder.Entity<PartidoHistorico>()
                .HasIndex(p => p.EquipoLocalId);

            modelBuilder.Entity<PartidoHistorico>()
                .HasIndex(p => p.EquipoVisitanteId);
        }
}
