using InterBook.API.Models;

namespace InterBook.API.Interfaces
{
    public interface IValidadorPassword
    {
        bool ValidarFormatoPassword(string password);
        List<string> ObtenerErroresPassword(string password);
    }

    public interface IValidadorEmail
    {
        bool ValidarFormatoEmail(string email);
    }

    public interface IServicioVerificacion
    {
        Task<string> GenerarCodigoVerificacion(string destino, string tipo);
        Task<bool> EnviarCodigoEmail(string email, string codigo);
        Task<bool> EnviarCodigoSMS(string telefono, string codigo);
    }

    public interface IRepositorioUsuario
    {
        Task<Usuario> ObtenerPorEmail(string email);
        Task<Usuario> ObtenerPorTelefono(string telefono);
        Task<bool> ActualizarIntentosLogin(int usuarioId, int intentos, DateTime? bloqueoHasta);
        Task<bool> RestablecerIntentosLogin(int usuarioId);
        Task<bool> GuardarIntentoLoginFallido(int usuarioId, string ipAddress, DateTime fecha);
    }

    public interface IServicioCifrado
    {
        string HashPassword(string password);
        bool VerificarPassword(string password, string hash);
    }

    public interface IServicioToken
    {
        string GenerarTokenJWT(Usuario usuario);
    }
}