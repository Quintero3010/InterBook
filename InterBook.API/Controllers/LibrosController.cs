using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InterBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibrosController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        public async Task<ActionResult<ListaPaginadaDto<LibroResponseDto>>> ObtenerLibros(
            [FromQuery] string? terminoBusqueda,
            [FromQuery] string? categoria,
            [FromQuery] string? ciudad,
            [FromQuery] bool soloDisponibles = true,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamañoPagina = 10)
        {
            try
            {
                var busqueda = new BusquedaLibroDto
                {
                    TerminoBusqueda = terminoBusqueda,
                    Categoria = categoria,
                    Ciudad = ciudad,
                    SoloDisponibles = soloDisponibles,
                    Pagina = pagina,
                    TamañoPagina = tamañoPagina
                };

                var resultado = await _libroService.ObtenerLibros(busqueda);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener libros", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LibroResponseDto>> ObtenerLibroPorId(int id)
        {
            try
            {
                var libro = await _libroService.ObtenerLibroPorId(id);

                if (libro == null)
                {
                    return NotFound(new { mensaje = "Libro no encontrado" });
                }

                return Ok(libro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el libro", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LibroResponseDto>> CrearLibro([FromBody] LibroDto libroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (usuarioId == 0)
                {
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });
                }

                var libro = await _libroService.CrearLibro(libroDto, usuarioId);
                return CreatedAtAction(nameof(ObtenerLibroPorId), new { id = libro.Id }, libro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el libro", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<LibroResponseDto>> ActualizarLibro(int id, [FromBody] LibroDto libroDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var libroExistente = await _libroService.ObtenerLibroPorId(id);

                if (libroExistente == null)
                {
                    return NotFound(new { mensaje = "Libro no encontrado" });
                }

                if (libroExistente.UsuarioId != usuarioId)
                {
                    return Forbid();
                }

                var libroActualizado = await _libroService.ActualizarLibro(id, libroDto);
                return Ok(libroActualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el libro", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> EliminarLibro(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var libro = await _libroService.ObtenerLibroPorId(id);

                if (libro == null)
                {
                    return NotFound(new { mensaje = "Libro no encontrado" });
                }

                if (libro.UsuarioId != usuarioId)
                {
                    return Forbid();
                }

                var resultado = await _libroService.EliminarLibro(id);

                if (!resultado)
                {
                    return BadRequest(new { mensaje = "No se puede eliminar el libro" });
                }

                return Ok(new { mensaje = "Libro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el libro", error = ex.Message });
            }
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<List<string>>> ObtenerCategorias()
        {
            try
            {
                var categorias = await _libroService.ObtenerCategorias();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener categorías", error = ex.Message });
            }
        }

        [HttpGet("mis-libros")]
        [Authorize]
        public async Task<ActionResult<List<LibroResponseDto>>> ObtenerMisLibros()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (usuarioId == 0)
                {
                    return Unauthorized(new { mensaje = "Usuario no autenticado" });
                }

                var libros = await _libroService.ObtenerLibrosPorUsuario(usuarioId);
                return Ok(libros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los libros", error = ex.Message });
            }
        }
    }
}