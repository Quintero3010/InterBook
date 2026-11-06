using InterBook.API.Data;
using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using InterBook.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InterBook.API.Services
{
    public class ServicioLogin 
    {
        private readonly InterBookDbContext _context;
        private readonly IServicioCifrado _servicioCifrado;

        public ServicioLogin(
            InterBookDbContext context,
            IServicioCifrado servicioCifrado)
        {
            _context = context;
            _servicioCifrado = servicioCifrado;
        }

        public async Task<(bool exito, string mensaje, Usuario? usuario)> Login(LoginDto loginDto)
        {
            try
            {
                // ✅ CORREGIDO: Usar loginDto.Email en lugar de EmailOTelefono
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Activo);

                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                // Verificar contraseña
                if (!_servicioCifrado.VerificarPassword(loginDto.Password ?? "", usuario.PasswordHash))
                {
                    return (false, "Contraseña incorrecta", null);
                }

                return (true, "Login exitoso", usuario);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }
    }
}