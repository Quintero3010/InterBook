using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InterBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntercambiosController : ControllerBase
    {
        private readonly IIntercambioService _intercambioService;

        public IntercambiosController(IIntercambioService intercambioService)
        {
            _intercambioService = intercambioService;
        }

        [HttpPost("solicitar")]
        public async Task<ActionResult> SolicitarIntercambio([FromBody] IntercambioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (usuarioId == 0)
                {
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });
                }

                var resultado = await _intercambioService.SolicitarIntercambio(dto, usuarioId);

                if (!resultado.Success)
                {
                    return BadRequest(new { mensaje = resultado.Message });
                }

                // NO devolver el objeto completo porque puede tener referencias circulares
                return Ok(new
                {
                    mensaje = resultado.Message,
                    intercambioId = resultado.Data?.Id
                });
            }
            catch (Exception ex)
            {
                // Log detallado del error
                var errorDetallado = new
                {
                    mensaje = "Error al solicitar intercambio",
                    error = ex.Message,
                    detalles = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace != null ? ex.StackTrace.Substring(0, Math.Min(500, ex.StackTrace.Length)) : ""
                };

                Console.WriteLine($"ERROR INTERCAMBIO: {System.Text.Json.JsonSerializer.Serialize(errorDetallado)}");

                return StatusCode(500, new {
                    mensaje = "Error al solicitar intercambio",
                    error = ex.Message,
                    detalles = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("responder")]
        public async Task<ActionResult> ResponderSolicitud([FromBody] RespuestaIntercambioDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    mensaje = "Datos inválidos",
                    errors = errores,
                    detalles = string.Join(", ", errores)
                });
            }

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await _intercambioService.ResponderSolicitud(dto, usuarioId);

                if (!resultado.Success)
                {
                    return BadRequest(new { mensaje = resultado.Message });
                }

                return Ok(new { mensaje = resultado.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR EN RESPONDER: {ex.Message}");
                Console.WriteLine($"INNER: {ex.InnerException?.Message}");

                return StatusCode(500, new {
                    mensaje = "Error al responder solicitud",
                    error = ex.Message,
                    detalles = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("{id}/completar")]
        public async Task<ActionResult> CompletarIntercambio(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await _intercambioService.CompletarIntercambio(id, usuarioId);

                if (!resultado.Success)
                {
                    return BadRequest(new { mensaje = resultado.Message });
                }

                return Ok(new { mensaje = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al completar intercambio", error = ex.Message });
            }
        }

        [HttpPost("{id}/cancelar")]
        public async Task<ActionResult> CancelarIntercambio(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var resultado = await _intercambioService.CancelarIntercambio(id, usuarioId);

                if (!resultado.Success)
                {
                    return BadRequest(new { mensaje = resultado.Message });
                }

                return Ok(new { mensaje = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cancelar intercambio", error = ex.Message });
            }
        }

        [HttpGet("mis-intercambios")]
        public async Task<ActionResult<List<IntercambioResponseDto>>> ObtenerMisIntercambios()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var intercambios = await _intercambioService.ObtenerIntercambiosPorUsuario(usuarioId);

                return Ok(intercambios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener intercambios", error = ex.Message });
            }
        }

        [HttpGet("pendientes")]
        public async Task<ActionResult<List<IntercambioResponseDto>>> ObtenerSolicitudesPendientes()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var solicitudes = await _intercambioService.ObtenerSolicitudesPendientes(usuarioId);

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener solicitudes pendientes", error = ex.Message });
            }
        }

        [HttpGet("enviadas")]
        public async Task<ActionResult<List<IntercambioResponseDto>>> ObtenerSolicitudesEnviadas()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var solicitudes = await _intercambioService.ObtenerSolicitudesEnviadas(usuarioId);

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener solicitudes enviadas", error = ex.Message });
            }
        }

        [HttpGet("historial")]
        public async Task<ActionResult<List<IntercambioResponseDto>>> ObtenerHistorial()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var historial = await _intercambioService.ObtenerHistorialIntercambios(usuarioId);

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener historial", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IntercambioResponseDto>> ObtenerIntercambioPorId(int id)
        {
            try
            {
                var intercambio = await _intercambioService.ObtenerIntercambioPorId(id);

                if (intercambio == null)
                {
                    return NotFound(new { mensaje = "Intercambio no encontrado" });
                }

                return Ok(intercambio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener intercambio", error = ex.Message });
            }
        }
    }
}