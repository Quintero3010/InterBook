using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterBook.API.Models
{
    [Table("Libros")]
    public class Libro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Propietario { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Autor { get; set; }

        [MaxLength(20)]
        public string ISBN { get; set; }

        [MaxLength(50)]
        public string Categoria { get; set; }

        public int? AñoPublicacion { get; set; }  // ← Con el ?

        [MaxLength(100)]
        public string Editorial { get; set; }

        [MaxLength(1000)]
        public string Descripcion { get; set; }

        [Required]
        [MaxLength(20)]
        public string EstadoFisico { get; set; } = "Buen Estado";

        [Required]
        [MaxLength(20)]
        public string EstadoDisponibilidad { get; set; } = "Disponible";

        [MaxLength(300)]
        public string ImagenUrl { get; set; }

        [MaxLength(100)]
        public string Ubicacion { get; set; }

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<Intercambio> IntercambiosComoOfrecido { get; set; }
        public virtual ICollection<Intercambio> IntercambiosComoSolicitado { get; set; }
    }
}