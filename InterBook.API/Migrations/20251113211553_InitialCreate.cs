using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InterBook.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Barrio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Biografia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FotoPerfilUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    EmailVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    TelefonoVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IntentosLoginFallidos = table.Column<int>(type: "integer", nullable: false),
                    BloqueoHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    IntercambiosRealizados = table.Column<int>(type: "integer", nullable: false),
                    CalificacionPromedio = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Autor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AñoPublicacion = table.Column<int>(type: "integer", nullable: true),
                    Editorial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EstadoFisico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EstadoDisponibilidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ImagenUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Ubicacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libros_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intercambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LibroOfrecidoId = table.Column<int>(type: "integer", nullable: false),
                    LibroSolicitadoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioSolicitanteId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioPropietarioId = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ComentarioSolicitante = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ComentarioPropietario = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LugarEncuentro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaEncuentro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intercambios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intercambios_Libros_LibroOfrecidoId",
                        column: x => x.LibroOfrecidoId,
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intercambios_Libros_LibroSolicitadoId",
                        column: x => x.LibroSolicitadoId,
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intercambios_Usuarios_UsuarioPropietarioId",
                        column: x => x.UsuarioPropietarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intercambios_Usuarios_UsuarioSolicitanteId",
                        column: x => x.UsuarioSolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Barrio", "Biografia", "BloqueoHasta", "CalificacionPromedio", "Ciudad", "Email", "EmailVerificado", "FechaRegistro", "FotoPerfilUrl", "IntentosLoginFallidos", "IntercambiosRealizados", "Nombre", "PasswordHash", "Rol", "Telefono", "TelefonoVerificado" },
                values: new object[,]
                {
                    { 1, true, "El Poblado", "Administrador oficial de la plataforma InterBook. Apasionado por los libros y la lectura.", null, 5, "Medellín", "admin@interbook.com", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://via.placeholder.com/150?text=Admin", 0, 0, "Administrador InterBook", "$2a$11$Kz3Q.9S2bM.FGsTvL0rIzOW5REEpmdLLoR4cv/RXf8.PaOWxSkcd6", "Administrador", "3001234567", true },
                    { 2, true, "Laureles", "Amante de la literatura clásica y contemporánea. Siempre buscando nuevas lecturas.", null, 5, "Medellín", "maria@test.com", true, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "https://via.placeholder.com/150?text=Maria", 0, 5, "María García", "$2a$11$DZQYTygpT6XoMWrWLYnJ0e8CwJRvX78Gc5s.N5mG.ol/jdQ3m0j.a", "Usuario", "3009876543", true },
                    { 3, true, "Envigado", "Desarrollador de software y lector entusiasta. Me encantan los libros de tecnología y ciencia ficción.", null, 4, "Medellín", "carlos@test.com", true, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "https://via.placeholder.com/150?text=Carlos", 0, 3, "Carlos Pérez", "$2a$11$KXmhUJH/uvRXjlCZYd8exevSnyBisWNojgj0Yfy6oXm/aSUOBLWyu", "Usuario", "3005554321", true }
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "Activo", "Autor", "AñoPublicacion", "Categoria", "Descripcion", "Editorial", "EstadoDisponibilidad", "EstadoFisico", "FechaPublicacion", "ISBN", "ImagenUrl", "Titulo", "Ubicacion", "UsuarioId" },
                values: new object[,]
                {
                    { 1, true, "Gabriel García Márquez", 1967, "Literatura Latinoamericana", "Edición especial en buen estado, sin rayones ni manchas", "Editorial Sudamericana", "Disponible", "Buen Estado", new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "978-0307474728", "https://via.placeholder.com/200x300?text=Cien+Años", "Cien Años de Soledad", "Medellín - Laureles", 2 },
                    { 2, true, "Antoine de Saint-Exupéry", 1943, "Infantil", "Edición ilustrada, como nuevo", "Reynal & Hitchcock", "Disponible", "Como Nuevo", new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), "978-0156012195", "https://via.placeholder.com/200x300?text=El+Principito", "El Principito", "Medellín - Laureles", 2 },
                    { 3, true, "Miguel de Cervantes", 1605, "Clásicos", "Edición completa, estado aceptable", "Francisco de Robles", "Disponible", "Aceptable", new DateTime(2024, 9, 15, 0, 0, 0, 0, DateTimeKind.Utc), "978-0060934347", "https://via.placeholder.com/200x300?text=Don+Quijote", "Don Quijote de la Mancha", "Medellín - Envigado", 3 },
                    { 4, true, "Robert C. Martin", 2008, "Tecnología", "Manual de programación en excelente estado", "Prentice Hall", "Disponible", "Buen Estado", new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "978-0132350884", "https://via.placeholder.com/200x300?text=Clean+Code", "Clean Code", "Medellín - Envigado", 3 },
                    { 5, true, "George Orwell", 1949, "Ficción Distópica", "Clásico de la literatura distópica, muy buen estado", "Secker & Warburg", "Disponible", "Buen Estado", new DateTime(2024, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), "978-0451524935", "https://via.placeholder.com/200x300?text=1984", "1984", "Medellín - Laureles", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_Estado",
                table: "Intercambios",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_FechaSolicitud",
                table: "Intercambios",
                column: "FechaSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_LibroOfrecidoId",
                table: "Intercambios",
                column: "LibroOfrecidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_LibroSolicitadoId",
                table: "Intercambios",
                column: "LibroSolicitadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_UsuarioPropietarioId",
                table: "Intercambios",
                column: "UsuarioPropietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Intercambios_UsuarioSolicitanteId_Estado",
                table: "Intercambios",
                columns: new[] { "UsuarioSolicitanteId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Autor",
                table: "Libros",
                column: "Autor");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_EstadoDisponibilidad",
                table: "Libros",
                column: "EstadoDisponibilidad");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_ISBN",
                table: "Libros",
                column: "ISBN");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Titulo",
                table: "Libros",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_UsuarioId",
                table: "Libros",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Telefono",
                table: "Usuarios",
                column: "Telefono",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intercambios");

            migrationBuilder.DropTable(
                name: "Libros");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
