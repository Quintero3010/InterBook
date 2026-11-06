using Microsoft.EntityFrameworkCore;
using InterBook.API.Models;

namespace InterBook.API.Data
{
    public class InterBookDbContext : DbContext
    {
        public InterBookDbContext(DbContextOptions<InterBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Intercambio> Intercambios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== CONFIGURACIÓN USUARIO =====
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Telefono).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Nombre).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Telefono).IsRequired();
            });

            // ===== CONFIGURACIÓN LIBRO =====
            modelBuilder.Entity<Libro>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Titulo);
                entity.HasIndex(e => e.Autor);
                entity.HasIndex(e => e.ISBN);
                entity.HasIndex(e => e.EstadoDisponibilidad);

                // Relación con Usuario (Propietario)
                entity.HasOne(l => l.Propietario)
                    .WithMany(u => u.LibrosPublicados)
                    .HasForeignKey(l => l.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relaciones con Intercambios
                entity.HasMany(l => l.IntercambiosComoOfrecido)
                    .WithOne(i => i.LibroOfrecido)
                    .HasForeignKey(i => i.LibroOfrecidoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(l => l.IntercambiosComoSolicitado)
                    .WithOne(i => i.LibroSolicitado)
                    .HasForeignKey(i => i.LibroSolicitadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN INTERCAMBIO =====
            modelBuilder.Entity<Intercambio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.FechaSolicitud);
                entity.HasIndex(e => new { e.UsuarioSolicitanteId, e.Estado });

                // Relación con Usuario Solicitante
                entity.HasOne(i => i.UsuarioSolicitante)
                    .WithMany(u => u.IntercambiosSolicitados)
                    .HasForeignKey(i => i.UsuarioSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Usuario Propietario
                entity.HasOne(i => i.UsuarioPropietario)
                    .WithMany(u => u.IntercambiosRecibidos)
                    .HasForeignKey(i => i.UsuarioPropietarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== DATOS INICIALES =====
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Usuarios de ejemplo
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Administrador InterBook",
                    Email = "admin@interbook.com",
                    Telefono = "3001234567",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Ciudad = "Medellín",
                    Barrio = "El Poblado",
                    Biografia = "Administrador oficial de la plataforma InterBook. Apasionado por los libros y la lectura.",
                    FotoPerfilUrl = "https://via.placeholder.com/150?text=Admin",
                    EmailVerificado = true,
                    TelefonoVerificado = true,
                    FechaRegistro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Rol = "Administrador",
                    Activo = true,
                    IntercambiosRealizados = 0,
                    CalificacionPromedio = 5
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "María García",
                    Email = "maria@test.com",
                    Telefono = "3009876543",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    Ciudad = "Medellín",
                    Barrio = "Laureles",
                    Biografia = "Amante de la literatura clásica y contemporánea. Siempre buscando nuevas lecturas.",
                    FotoPerfilUrl = "https://via.placeholder.com/150?text=Maria",
                    EmailVerificado = true,
                    TelefonoVerificado = true,
                    FechaRegistro = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    Rol = "Usuario",
                    Activo = true,
                    IntercambiosRealizados = 5,
                    CalificacionPromedio = 5
                },
                new Usuario
                {
                    Id = 3,
                    Nombre = "Carlos Pérez",
                    Email = "carlos@test.com",
                    Telefono = "3005554321",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    Ciudad = "Medellín",
                    Barrio = "Envigado",
                    Biografia = "Desarrollador de software y lector entusiasta. Me encantan los libros de tecnología y ciencia ficción.",
                    FotoPerfilUrl = "https://via.placeholder.com/150?text=Carlos",
                    EmailVerificado = true,
                    TelefonoVerificado = true,
                    FechaRegistro = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                    Rol = "Usuario",
                    Activo = true,
                    IntercambiosRealizados = 3,
                    CalificacionPromedio = 4
                }
            );

            // Libros de ejemplo para intercambio
            modelBuilder.Entity<Libro>().HasData(
                new Libro
                {
                    Id = 1,
                    UsuarioId = 2,
                    Titulo = "Cien Años de Soledad",
                    Autor = "Gabriel García Márquez",
                    ISBN = "978-0307474728",
                    Categoria = "Literatura Latinoamericana",
                    AñoPublicacion = 1967,
                    Editorial = "Editorial Sudamericana",
                    Descripcion = "Edición especial en buen estado, sin rayones ni manchas",
                    EstadoFisico = "Buen Estado",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = "https://via.placeholder.com/200x300?text=Cien+Años",
                    Ubicacion = "Medellín - Laureles",
                    FechaPublicacion = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                },
                new Libro
                {
                    Id = 2,
                    UsuarioId = 2,
                    Titulo = "El Principito",
                    Autor = "Antoine de Saint-Exupéry",
                    ISBN = "978-0156012195",
                    Categoria = "Infantil",
                    AñoPublicacion = 1943,
                    Editorial = "Reynal & Hitchcock",
                    Descripcion = "Edición ilustrada, como nuevo",
                    EstadoFisico = "Como Nuevo",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = "https://via.placeholder.com/200x300?text=El+Principito",
                    Ubicacion = "Medellín - Laureles",
                    FechaPublicacion = new DateTime(2024, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                },
                new Libro
                {
                    Id = 3,
                    UsuarioId = 3,
                    Titulo = "Don Quijote de la Mancha",
                    Autor = "Miguel de Cervantes",
                    ISBN = "978-0060934347",
                    Categoria = "Clásicos",
                    AñoPublicacion = 1605,
                    Editorial = "Francisco de Robles",
                    Descripcion = "Edición completa, estado aceptable",
                    EstadoFisico = "Aceptable",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = "https://via.placeholder.com/200x300?text=Don+Quijote",
                    Ubicacion = "Medellín - Envigado",
                    FechaPublicacion = new DateTime(2024, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                },
                new Libro
                {
                    Id = 4,
                    UsuarioId = 3,
                    Titulo = "Clean Code",
                    Autor = "Robert C. Martin",
                    ISBN = "978-0132350884",
                    Categoria = "Tecnología",
                    AñoPublicacion = 2008,
                    Editorial = "Prentice Hall",
                    Descripcion = "Manual de programación en excelente estado",
                    EstadoFisico = "Buen Estado",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = "https://via.placeholder.com/200x300?text=Clean+Code",
                    Ubicacion = "Medellín - Envigado",
                    FechaPublicacion = new DateTime(2024, 10, 10, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                },
                new Libro
                {
                    Id = 5,
                    UsuarioId = 2,
                    Titulo = "1984",
                    Autor = "George Orwell",
                    ISBN = "978-0451524935",
                    Categoria = "Ficción Distópica",
                    AñoPublicacion = 1949,
                    Editorial = "Secker & Warburg",
                    Descripcion = "Clásico de la literatura distópica, muy buen estado",
                    EstadoFisico = "Buen Estado",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = "https://via.placeholder.com/200x300?text=1984",
                    Ubicacion = "Medellín - Laureles",
                    FechaPublicacion = new DateTime(2024, 10, 20, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                }
            );
        }
    }
}