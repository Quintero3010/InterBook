using InterBook.API.DTOs;
using InterBook.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterBook.API.Interfaces
{
    public interface IIntercambioService
    {
        Task<(bool Success, string Message, Intercambio Data)> SolicitarIntercambio(IntercambioDto dto, int usuarioSolicitanteId);
        Task<(bool Success, string Message)> ResponderSolicitud(RespuestaIntercambioDto dto, int usuarioPropietarioId);
        Task<(bool Success, string Message)> CompletarIntercambio(int intercambioId, int usuarioId);
        Task<(bool Success, string Message)> CancelarIntercambio(int intercambioId, int usuarioId);
        Task<List<IntercambioResponseDto>> ObtenerIntercambiosPorUsuario(int usuarioId);
        Task<List<IntercambioResponseDto>> ObtenerSolicitudesPendientes(int usuarioId);
        Task<List<IntercambioResponseDto>> ObtenerSolicitudesEnviadas(int usuarioId);
        Task<List<IntercambioResponseDto>> ObtenerHistorialIntercambios(int usuarioId);
        Task<IntercambioResponseDto> ObtenerIntercambioPorId(int intercambioId);
        Task<bool> ExisteSolicitudPendiente(int libroOfrecidoId, int libroSolicitadoId);
    }
}