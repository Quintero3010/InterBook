using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InterBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Exitoso = false,
                    Mensaje = "Datos inválidos",
                    Errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var resultado = await _authService.Login(loginDto);

            if (!resultado.Exitoso)
            {
                return Unauthorized(resultado);
            }

            return Ok(resultado);
        }

        [HttpPost("registro")]
        public async Task<ActionResult<AuthResponseDto>> Registro([FromBody] RegistroDto registroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponseDto
                {
                    Exitoso = false,
                    Mensaje = "Datos inválidos",
                    Errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var resultado = await _authService.Registro(registroDto);

            if (!resultado.Exitoso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpPost("cambiar-password")]
        [Authorize]
        public async Task<ActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensaje = "Datos inválidos" });
            }

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var resultado = await _authService.CambiarPassword(usuarioId, dto);

            if (!resultado)
            {
                return BadRequest(new { mensaje = "Error al cambiar la contraseña" });
            }

            return Ok(new { mensaje = "Contraseña cambiada exitosamente" });
        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> ObtenerPerfil()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = await _authService.ObtenerUsuarioPorId(usuarioId);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            return Ok(new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Ciudad = usuario.Ciudad,
                EmailVerificado = usuario.EmailVerificado,
                IntercambiosRealizados = usuario.IntercambiosRealizados
            });
        }

        [HttpGet("verificar")]
        [Authorize]
        public ActionResult VerificarToken()
        {
            return Ok(new { mensaje = "Token válido", autenticado = true });
        }
    }
}