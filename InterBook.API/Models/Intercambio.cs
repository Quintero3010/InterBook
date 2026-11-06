using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterBook.API.Models
{
    [Table("Intercambios")]
    public class Intercambio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int LibroOfrecidoId { get; set; }

        [ForeignKey("LibroOfrecidoId")]
        public virtual Libro LibroOfrecido { get; set; }

        [Required]
        public int LibroSolicitadoId { get; set; }

        [ForeignKey("LibroSolicitadoId")]
        public virtual Libro LibroSolicitado { get; set; }

        [Required]
        public int UsuarioSolicitanteId { get; set; }

        [ForeignKey("UsuarioSolicitanteId")]
        public virtual Usuario UsuarioSolicitante { get; set; }

        [Required]
        public int UsuarioPropietarioId { get; set; }

        [ForeignKey("UsuarioPropietarioId")]
        public virtual Usuario UsuarioPropietario { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public DateTime? FechaRespuesta { get; set; }
        public DateTime? FechaCompletado { get; set; }

        [MaxLength(500)]
        public string ComentarioSolicitante { get; set; }

        [MaxLength(500)]
        public string ComentarioPropietario { get; set; }

        [MaxLength(200)]
        public string LugarEncuentro { get; set; }
        public DateTime? FechaEncuentro { get; set; }
        public bool Activo { get; set; } = true;
    }
}