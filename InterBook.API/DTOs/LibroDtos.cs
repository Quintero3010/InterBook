using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterBook.API.DTOs
{
    public class LibroDto
    {
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Autor { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [MaxLength(50)]
        public string? Categoria { get; set; }

        public int? AñoPublicacion { get; set; }

        [MaxLength(100)]
        public string? Editorial { get; set; }

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [RegularExpression("^(Nuevo|Como Nuevo|Buen Estado|Aceptable|Desgastado)$")]
        public string EstadoFisico { get; set; } = "Buen Estado";

        [MaxLength(300)]
        public string? ImagenUrl { get; set; }

        [MaxLength(100)]
        public string? Ubicacion { get; set; }
    }

    public class LibroResponseDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? NombrePropietario { get; set; }
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public string? ISBN { get; set; }
        public string? Categoria { get; set; }
        public int? AñoPublicacion { get; set; }
        public string? Editorial { get; set; }
        public string? Descripcion { get; set; }
        public string? EstadoFisico { get; set; }
        public string? EstadoDisponibilidad { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Activo { get; set; }
    }

    public class BusquedaLibroDto
    {
        public string? TerminoBusqueda { get; set; }
        public string? Categoria { get; set; }
        public string? Ciudad { get; set; }
        public bool SoloDisponibles { get; set; } = false;
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }

    public class ListaPaginadaDto<T>
    {
        public List<T> Elementos { get; set; } = new();
        public int TotalElementos { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
    }
}