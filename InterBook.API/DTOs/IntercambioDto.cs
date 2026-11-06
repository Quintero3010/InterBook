using System;
using System.ComponentModel.DataAnnotations;

namespace InterBook.API.DTOs
{
    // DTO para crear una solicitud de intercambio
    public class IntercambioDto
    {
        [Required]
        public int LibroOfrecidoId { get; set; }

        [Required]
        public int LibroSolicitadoId { get; set; }

        [MaxLength(500)]
        public string? ComentarioSolicitante { get; set; }

        [MaxLength(200)]
        public string? LugarEncuentro { get; set; }

        public DateTime? FechaEncuentro { get; set; }
    }

    // DTO para responder a una solicitud
    public class RespuestaIntercambioDto
    {
        [Required]
        public int IntercambioId { get; set; }

        [Required]
        [RegularExpression("^(Aceptado|Rechazado)$", ErrorMessage = "Estado debe ser 'Aceptado' o 'Rechazado'")]
        public string Estado { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ComentarioPropietario { get; set; }

        [MaxLength(200)]
        public string? LugarEncuentro { get; set; }

        public DateTime? FechaEncuentro { get; set; }
    }

    // DTO para completar un intercambio
    public class CompletarIntercambioDto
    {
        [Required]
        public int IntercambioId { get; set; }
        public bool Completado { get; set; } = true;
    }

    // DTO de respuesta completa
    public class IntercambioResponseDto
    {
        public int Id { get; set; }
        public LibroBasicoDto LibroOfrecido { get; set; } = null!;
        public LibroBasicoDto LibroSolicitado { get; set; } = null!;
        public UsuarioBasicoDto UsuarioSolicitante { get; set; } = null!;
        public UsuarioBasicoDto UsuarioPropietario { get; set; } = null!;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public DateTime? FechaCompletado { get; set; }
        public string? ComentarioSolicitante { get; set; }
        public string? ComentarioPropietario { get; set; }
        public string? LugarEncuentro { get; set; }
        public DateTime? FechaEncuentro { get; set; }
    }

    // DTOs básicos
    public class LibroBasicoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public string EstadoFisico { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public string EstadoDisponibilidad { get; set; } = string.Empty;
    }

    public class UsuarioBasicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Ciudad { get; set; }
        public int IntercambiosRealizados { get; set; }
        public int CalificacionPromedio { get; set; }
    }
}