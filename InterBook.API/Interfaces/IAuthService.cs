using InterBook.API.DTOs;
using InterBook.API.Models;

namespace InterBook.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<AuthResponseDto> Registro(RegistroDto registroDto);
        Task<bool> CambiarPassword(int usuarioId, CambiarPasswordDto cambiarPasswordDto);
        Task<Usuario> ObtenerUsuarioPorId(int id);
        Task<Usuario> ObtenerUsuarioPorEmail(string email);
    }
}
