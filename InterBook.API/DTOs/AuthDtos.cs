using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterBook.API.DTOs
{
    // ========== DTO DE RESPUESTA ==========
    public class AuthResponseDto
    {
        public bool Exitoso { get; set; }
        public string? Mensaje { get; set; }
        public string? Token { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public List<string> Errores { get; set; } = new();
    }

    // ========== DTO DE USUARIO ==========
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Rol { get; set; }
        public string? Ciudad { get; set; }
        public bool EmailVerificado { get; set; }
        public int IntercambiosRealizados { get; set; }
    }

    // ========== CAMBIAR PASSWORD DTO ==========
    public class CambiarPasswordDto
    {
        [Required]
        public string? PasswordActual { get; set; }

        [Required]
        [MinLength(8)]
        public string? PasswordNueva { get; set; }

        [Required]
        [Compare("PasswordNueva")]
        public string? ConfirmarPassword { get; set; }
    }

    // ========== LOGIN DTO (CORREGIDO) ==========
    public class LoginDto
    {
        [Required]
        public string? Email { get; set; }
        
        [Required]
        public string? Password { get; set; }
    }

    // ========== REGISTRO DTO (CORREGIDO Y COMPLETO) ==========
    public class RegistroDto
    {
        [Required]
        public string? Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
     
        [Required]
     [Phone]
     public string? Telefono { get; set; }

        [Required]
        public string? Ciudad { get; set; }

        public string? Barrio { get; set; }
        
        [Required]
        [MinLength(8)]
   public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}