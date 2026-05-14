using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProcessingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, Rol, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios => Set<Usuarios>();
        public DbSet<Oficina> Oficinas => Set<Oficina>();
        public DbSet<Expediente> Expedientes => Set<Expediente>();
        public DbSet<DocumentoArchivo> DocumentoArchivos => Set<DocumentoArchivo>();
        public DbSet<Movimiento> Movimientos => Set<Movimiento>();
        public DbSet<TipoDocumento> TipoDocumentos => Set<TipoDocumento>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var ADMIN_ID = Guid.Parse("f22687c4-279e-473d-829d-476775988e40");
            var ROLE_ADMIN = Guid.Parse("44598992-04e4-44b2-a42c-29b110756770");
            var ROLE_USER_EXTERN = Guid.Parse("54598992-05e7-44b2-a42c-29b177756770");
            var ROLE_USER_STAFF = Guid.Parse("93598992-27e6-32ef-a42c-29b110754970");

            builder.Entity<Rol>().HasData(
                new Rol { Id = ROLE_ADMIN, Name = "Admin", NormalizedName = "ADMIN" },
                new Rol { Id = ROLE_USER_EXTERN, Name = "Ciudadano", NormalizedName = "CIUDADANO"},
                new Rol { Id = ROLE_USER_STAFF, Name = "Personal", NormalizedName = "PERSONAL"}
            );

            var adminUser = new IdentityUser<Guid>
            {
                Id = ADMIN_ID,
                UserName = "admin@ejemplo.gob.pe",
                NormalizedUserName = "ADMIN@EJEMPLO.GOB.PE",
                Email = "admin@ejemplo.gob.pe",
                NormalizedEmail = "ADMIN@EJEMPLO.GOB.PE",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<IdentityUser<Guid>>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123*");

            //builder.Entity<Usuarios>().HasData(adminUser);

            builder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    RoleId = ROLE_ADMIN,
                    UserId = ADMIN_ID
                }
            );

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var idProperty = entityType.FindProperty("Id");
                if(idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.SetDefaultValueSql("NEWID()");
                }
            }

            builder.Entity<IdentityUser<Guid>>().HasData(adminUser);

            builder.Entity<Movimiento>(entity =>
            {
                entity.HasOne(m => m.OficinaOrigen).WithMany().HasForeignKey(m => m.OficinaOrigenId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(m => m.OficinaDestino).WithMany().HasForeignKey(m => m.OficinaDestinoId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(m => m.Expediente).WithMany(e => e.Historial).HasForeignKey(m => m.ExpedienteId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Expediente>(entity =>
            {
                entity.HasIndex(e => e.NumeroExpediente).IsUnique();
                entity.HasOne(e => e.Usuario).WithMany().HasForeignKey(e => e.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Oficina>().HasOne(o => o.OficinaPadre).WithMany().HasForeignKey(o => o.OficinaPadreId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Usuarios>().HasIndex(u => u.Dni).IsUnique();
        }
    }
}
