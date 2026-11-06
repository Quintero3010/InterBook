using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InterBook.API.Data;

namespace InterBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly InterBookDbContext _context;

        public DatabaseController(InterBookDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ENDPOINT TEMPORAL: Resetea los estados de los libros a "Disponible"
        /// y elimina todos los intercambios pendientes
        /// </summary>
    [HttpPost("reset-estados")]
        public async Task<IActionResult> ResetearEstados()
        {
       try
{
              // 1. Obtener todos los libros con estados problemáticos
        var librosReserved = await _context.Libros
     .Where(l => l.EstadoDisponibilidad == "Reservado" || 
        l.EstadoDisponibilidad == "En Intercambio")
.ToListAsync();

       // 2. Cambiar todos a "Disponible"
           foreach (var libro in librosReserved)
                {
        libro.EstadoDisponibilidad = "Disponible";
    }

      // 3. Eliminar intercambios pendientes (opcional - comentado por seguridad)
    // var intercambiosPendientes = await _context.Intercambios
     //     .Where(i => i.Estado == "Pendiente")
        //     .ToListAsync();
       // _context.Intercambios.RemoveRange(intercambiosPendientes);

                // 4. Guardar cambios
    await _context.SaveChangesAsync();

          return Ok(new
    {
       mensaje = "Estados reseteados correctamente",
  librosActualizados = librosReserved.Count,
    estadoFinal = await _context.Libros
         .Select(l => new { l.Id, l.Titulo, l.EstadoDisponibilidad })
   .ToListAsync()
        });
        }
    catch (Exception ex)
            {
     return StatusCode(500, new
        {
mensaje = "Error al resetear estados",
           error = ex.Message,
          inner = ex.InnerException?.Message
      });
    }
        }

      /// <summary>
        /// ENDPOINT TEMPORAL: Muestra el estado actual de todos los libros
        /// </summary>
        [HttpGet("ver-estados")]
    public async Task<IActionResult> VerEstados()
        {
    try
 {
       var libros = await _context.Libros
          .Select(l => new
                  {
           l.Id,
   l.Titulo,
             l.EstadoDisponibilidad,
   Propietario = l.Propietario.Nombre,
                 l.UsuarioId
     })
               .ToListAsync();

    var intercambios = await _context.Intercambios
               .Select(i => new
            {
         i.Id,
      i.Estado,
     i.FechaSolicitud,
      LibroOfrecido = i.LibroOfrecido.Titulo,
      LibroSolicitado = i.LibroSolicitado.Titulo,
       Solicitante = i.UsuarioSolicitante.Nombre,
            Propietario = i.UsuarioPropietario.Nombre
              })
               .ToListAsync();

                return Ok(new
         {
     libros,
        totalLibros = libros.Count,
librosDisponibles = libros.Count(l => l.EstadoDisponibilidad == "Disponible"),
              librosReservados = libros.Count(l => l.EstadoDisponibilidad == "Reservado"),
           intercambios,
   totalIntercambios = intercambios.Count
    });
     }
       catch (Exception ex)
            {
    return StatusCode(500, new
                {
      mensaje = "Error al obtener estados",
       error = ex.Message
   });
   }
    }

        /// <summary>
    /// ENDPOINT TEMPORAL: Elimina TODOS los intercambios (usar con cuidado)
        /// </summary>
        [HttpDelete("limpiar-intercambios")]
   public async Task<IActionResult> LimpiarIntercambios()
        {
        try
         {
     var intercambios = await _context.Intercambios.ToListAsync();
        _context.Intercambios.RemoveRange(intercambios);
        await _context.SaveChangesAsync();

              return Ok(new
     {
            mensaje = "Intercambios eliminados correctamente",
         intercambiosEliminados = intercambios.Count
                });
      }
            catch (Exception ex)
 {
return StatusCode(500, new
      {
            mensaje = "Error al limpiar intercambios",
      error = ex.Message
        });
}
        }
    }
}
