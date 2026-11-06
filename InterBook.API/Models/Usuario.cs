using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterBook.API.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(20)]
        public string Rol { get; set; } = "Usuario";

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(100)]
        public string Ciudad { get; set; }

        [MaxLength(100)]
        public string Barrio { get; set; }

        [MaxLength(500)]
        public string Biografia { get; set; }

        [MaxLength(300)]
        public string FotoPerfilUrl { get; set; }

        public bool EmailVerificado { get; set; } = false;

        public bool TelefonoVerificado { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int IntentosLoginFallidos { get; set; } = 0;

        public DateTime? BloqueoHasta { get; set; }

        public bool Activo { get; set; } = true;

        public int IntercambiosRealizados { get; set; } = 0;

        public int CalificacionPromedio { get; set; } = 0;

        // Relaciones
        public virtual ICollection<Libro> LibrosPublicados { get; set; }
        public virtual ICollection<Intercambio> IntercambiosSolicitados { get; set; }
        public virtual ICollection<Intercambio> IntercambiosRecibidos { get; set; }
    }
}