using InterBook.API.Data;
using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using InterBook.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterBook.API.Services
{
    public class IntercambioService : IIntercambioService
    {
        private readonly InterBookDbContext _context;

        public IntercambioService(InterBookDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Intercambio Data)> SolicitarIntercambio(
            IntercambioDto dto, int usuarioSolicitanteId)
        {
            try
            {
                var libroOfrecido = await _context.Libros
                    .Include(l => l.Propietario)
                    .FirstOrDefaultAsync(l => l.Id == dto.LibroOfrecidoId);

                var libroSolicitado = await _context.Libros
                    .Include(l => l.Propietario)
                    .FirstOrDefaultAsync(l => l.Id == dto.LibroSolicitadoId);

                if (libroOfrecido == null || libroSolicitado == null)
                    return (false, "Uno o ambos libros no existen", null);

                if (libroOfrecido.UsuarioId != usuarioSolicitanteId)
                    return (false, "No eres el propietario del libro ofrecido", null);

                if (libroSolicitado.UsuarioId == usuarioSolicitanteId)
                    return (false, "No puedes intercambiar libros contigo mismo", null);

                if (libroOfrecido.EstadoDisponibilidad != "Disponible")
                    return (false, $"Tu libro '{libroOfrecido.Titulo}' no está disponible para intercambio (Estado actual: {libroOfrecido.EstadoDisponibilidad})", null);

                if (libroSolicitado.EstadoDisponibilidad != "Disponible")
                    return (false, $"El libro solicitado '{libroSolicitado.Titulo}' ya no está disponible (Estado actual: {libroSolicitado.EstadoDisponibilidad})", null);

                var solicitudExistente = await _context.Intercambios.AnyAsync(i =>
                    i.LibroOfrecidoId == dto.LibroOfrecidoId &&
                    i.LibroSolicitadoId == dto.LibroSolicitadoId &&
                    i.Estado == "Pendiente");

                if (solicitudExistente)
                    return (false, "Ya existe una solicitud pendiente para estos libros", null);

                var intercambio = new Intercambio
                {
                    LibroOfrecidoId = dto.LibroOfrecidoId,
                    LibroSolicitadoId = dto.LibroSolicitadoId,
                    UsuarioSolicitanteId = usuarioSolicitanteId,
                    UsuarioPropietarioId = libroSolicitado.UsuarioId,
                    Estado = "Pendiente",
                    FechaSolicitud = DateTime.UtcNow,
                    ComentarioSolicitante = dto.ComentarioSolicitante,
                    ComentarioPropietario = "", // ← FIX TEMPORAL: string vacío en lugar de null
                    LugarEncuentro = dto.LugarEncuentro,
                    FechaEncuentro = dto.FechaEncuentro,
                    Activo = true
                };

                _context.Intercambios.Add(intercambio);
                // NO cambiar el estado hasta que se acepte
                // libroOfrecido.EstadoDisponibilidad = "En Intercambio";
                // libroSolicitado.EstadoDisponibilidad = "En Intercambio";

                await _context.SaveChangesAsync();
                return (true, "Solicitud de intercambio enviada exitosamente", intercambio);
            }
            catch (Exception ex)
            {
                return (false, $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> ResponderSolicitud(
            RespuestaIntercambioDto dto, int usuarioPropietarioId)
        {
            try
            {
                var intercambio = await _context.Intercambios
                    .Include(i => i.LibroOfrecido)
                    .Include(i => i.LibroSolicitado)
                    .FirstOrDefaultAsync(i => i.Id == dto.IntercambioId);

                if (intercambio == null)
                    return (false, "Solicitud no encontrada");

                if (intercambio.UsuarioPropietarioId != usuarioPropietarioId)
                    return (false, "No tienes permisos");

                if (intercambio.Estado != "Pendiente")
                    return (false, "Esta solicitud ya fue respondida");

                intercambio.Estado = dto.Estado;
                intercambio.FechaRespuesta = DateTime.UtcNow;
                intercambio.ComentarioPropietario = dto.ComentarioPropietario;

                if (dto.Estado == "Aceptado")
                {
                    intercambio.LugarEncuentro = dto.LugarEncuentro ?? intercambio.LugarEncuentro;
                    intercambio.FechaEncuentro = dto.FechaEncuentro ?? intercambio.FechaEncuentro;
                    // Cambiar estado de libros al aceptar
                    intercambio.LibroOfrecido.EstadoDisponibilidad = "Reservado";
                    intercambio.LibroSolicitado.EstadoDisponibilidad = "Reservado";
                }
                else if (dto.Estado == "Rechazado")
                {
                    // Asegurar que los libros vuelvan a estar disponibles
                    intercambio.LibroOfrecido.EstadoDisponibilidad = "Disponible";
                    intercambio.LibroSolicitado.EstadoDisponibilidad = "Disponible";
                }

                await _context.SaveChangesAsync();
                return (true, dto.Estado == "Aceptado" ? "Solicitud aceptada" : "Solicitud rechazada");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CompletarIntercambio(int intercambioId, int usuarioId)
        {
            try
            {
                var intercambio = await _context.Intercambios
                    .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                    .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                    .FirstOrDefaultAsync(i => i.Id == intercambioId);

                if (intercambio == null)
                    return (false, "Intercambio no encontrado");

                if (intercambio.UsuarioSolicitanteId != usuarioId &&
                    intercambio.UsuarioPropietarioId != usuarioId)
                    return (false, "No tienes permisos");

                if (intercambio.Estado != "Aceptado")
                    return (false, "Solo se pueden completar intercambios aceptados");

                intercambio.Estado = "Completado";
                intercambio.FechaCompletado = DateTime.UtcNow;
                intercambio.LibroOfrecido.EstadoDisponibilidad = "Intercambiado";
                intercambio.LibroSolicitado.EstadoDisponibilidad = "Intercambiado";
                intercambio.LibroOfrecido.Propietario.IntercambiosRealizados++;
                intercambio.LibroSolicitado.Propietario.IntercambiosRealizados++;

                await _context.SaveChangesAsync();
                return (true, "Intercambio completado exitosamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CancelarIntercambio(int intercambioId, int usuarioId)
        {
            try
            {
                var intercambio = await _context.Intercambios
                    .Include(i => i.LibroOfrecido)
                    .Include(i => i.LibroSolicitado)
                    .FirstOrDefaultAsync(i => i.Id == intercambioId);

                if (intercambio == null)
                    return (false, "Intercambio no encontrado");

                if (intercambio.UsuarioSolicitanteId != usuarioId)
                    return (false, "Solo el solicitante puede cancelar");

                if (intercambio.Estado == "Completado")
                    return (false, "No se puede cancelar un intercambio completado");

                intercambio.Estado = "Cancelado";
                intercambio.Activo = false;
                intercambio.LibroOfrecido.EstadoDisponibilidad = "Disponible";
                intercambio.LibroSolicitado.EstadoDisponibilidad = "Disponible";

                await _context.SaveChangesAsync();
                return (true, "Intercambio cancelado");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<List<IntercambioResponseDto>> ObtenerIntercambiosPorUsuario(int usuarioId)
        {
            var intercambios = await _context.Intercambios
                .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                .Include(i => i.UsuarioSolicitante)
                .Include(i => i.UsuarioPropietario)
                .Where(i => i.UsuarioSolicitanteId == usuarioId || i.UsuarioPropietarioId == usuarioId)
                .Where(i => i.Activo)
                .OrderByDescending(i => i.FechaSolicitud)
                .ToListAsync();

            return MapToDto(intercambios);
        }

        public async Task<List<IntercambioResponseDto>> ObtenerSolicitudesPendientes(int usuarioId)
        {
            var intercambios = await _context.Intercambios
                .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                .Include(i => i.UsuarioSolicitante)
                .Include(i => i.UsuarioPropietario)
                .Where(i => i.UsuarioPropietarioId == usuarioId && i.Estado == "Pendiente")
                .OrderByDescending(i => i.FechaSolicitud)
                .ToListAsync();

            return MapToDto(intercambios);
        }

        public async Task<List<IntercambioResponseDto>> ObtenerSolicitudesEnviadas(int usuarioId)
        {
            var intercambios = await _context.Intercambios
                .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                .Include(i => i.UsuarioSolicitante)
                .Include(i => i.UsuarioPropietario)
                .Where(i => i.UsuarioSolicitanteId == usuarioId)
                .OrderByDescending(i => i.FechaSolicitud)
                .ToListAsync();

            return MapToDto(intercambios);
        }

        public async Task<List<IntercambioResponseDto>> ObtenerHistorialIntercambios(int usuarioId)
        {
            var intercambios = await _context.Intercambios
                .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                .Include(i => i.UsuarioSolicitante)
                .Include(i => i.UsuarioPropietario)
                .Where(i => (i.UsuarioSolicitanteId == usuarioId || i.UsuarioPropietarioId == usuarioId)
                    && (i.Estado == "Completado" || i.Estado == "Rechazado" || i.Estado == "Cancelado"))
                .OrderByDescending(i => i.FechaCompletado ?? i.FechaRespuesta ?? i.FechaSolicitud)
                .ToListAsync();

            return MapToDto(intercambios);
        }

        public async Task<IntercambioResponseDto> ObtenerIntercambioPorId(int intercambioId)
        {
            var intercambio = await _context.Intercambios
                .Include(i => i.LibroOfrecido).ThenInclude(l => l.Propietario)
                .Include(i => i.LibroSolicitado).ThenInclude(l => l.Propietario)
                .Include(i => i.UsuarioSolicitante)
                .Include(i => i.UsuarioPropietario)
                .FirstOrDefaultAsync(i => i.Id == intercambioId);

            return intercambio != null ? MapToDto(new List<Intercambio> { intercambio }).First() : null;
        }

        public async Task<bool> ExisteSolicitudPendiente(int libroOfrecidoId, int libroSolicitadoId)
        {
            return await _context.Intercambios.AnyAsync(i =>
                i.LibroOfrecidoId == libroOfrecidoId &&
                i.LibroSolicitadoId == libroSolicitadoId &&
                i.Estado == "Pendiente");
        }

        private List<IntercambioResponseDto> MapToDto(List<Intercambio> intercambios)
        {
            return intercambios.Select(i => new IntercambioResponseDto
            {
                Id = i.Id,
                LibroOfrecido = new LibroBasicoDto
                {
                    Id = i.LibroOfrecido.Id,
                    Titulo = i.LibroOfrecido.Titulo,
                    Autor = i.LibroOfrecido.Autor,
                    ImagenUrl = i.LibroOfrecido.ImagenUrl,
                    EstadoFisico = i.LibroOfrecido.EstadoFisico,
                    Ubicacion = i.LibroOfrecido.Ubicacion,
                    EstadoDisponibilidad = i.LibroOfrecido.EstadoDisponibilidad
                },
                LibroSolicitado = new LibroBasicoDto
                {
                    Id = i.LibroSolicitado.Id,
                    Titulo = i.LibroSolicitado.Titulo,
                    Autor = i.LibroSolicitado.Autor,
                    ImagenUrl = i.LibroSolicitado.ImagenUrl,
                    EstadoFisico = i.LibroSolicitado.EstadoFisico,
                    Ubicacion = i.LibroSolicitado.Ubicacion,
                    EstadoDisponibilidad = i.LibroSolicitado.EstadoDisponibilidad
                },
                UsuarioSolicitante = new UsuarioBasicoDto
                {
                    Id = i.UsuarioSolicitante.Id,
                    Nombre = i.UsuarioSolicitante.Nombre,
                    Email = i.UsuarioSolicitante.Email,
                    Telefono = i.UsuarioSolicitante.Telefono,
                    Ciudad = i.UsuarioSolicitante.Ciudad,
                    IntercambiosRealizados = i.UsuarioSolicitante.IntercambiosRealizados,
                    CalificacionPromedio = i.UsuarioSolicitante.CalificacionPromedio
                },
                UsuarioPropietario = new UsuarioBasicoDto
                {
                    Id = i.UsuarioPropietario.Id,
                    Nombre = i.UsuarioPropietario.Nombre,
                    Email = i.UsuarioPropietario.Email,
                    Telefono = i.UsuarioPropietario.Telefono,
                    Ciudad = i.UsuarioPropietario.Ciudad,
                    IntercambiosRealizados = i.UsuarioPropietario.IntercambiosRealizados,
                    CalificacionPromedio = i.UsuarioPropietario.CalificacionPromedio
                },
                Estado = i.Estado,
                FechaSolicitud = i.FechaSolicitud,
                FechaRespuesta = i.FechaRespuesta,
                FechaCompletado = i.FechaCompletado,
                ComentarioSolicitante = i.ComentarioSolicitante,
                ComentarioPropietario = i.ComentarioPropietario,
                LugarEncuentro = i.LugarEncuentro,
                FechaEncuentro = i.FechaEncuentro
            }).ToList();
        }
    }
}