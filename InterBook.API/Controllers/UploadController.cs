using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterBook.API.Controllers
{
    [Route("api/[controller]")]
 [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
      _environment = environment;
            _logger = logger;
        }

     [HttpPost("imagen-libro")]
        [Authorize]
        public async Task<IActionResult> SubirImagenLibro(IFormFile imagen)
        {
      try
   {
     if (imagen == null || imagen.Length == 0)
      {
           return BadRequest(new { mensaje = "No se ha seleccionado ninguna imagen" });
       }

  // Validar tipo de archivo
           var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
       var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
     {
             return BadRequest(new { mensaje = "Solo se permiten imágenes (jpg, jpeg, png, gif, webp)" });
     }

           // Validar tamaño (máximo 5MB)
          if (imagen.Length > 5 * 1024 * 1024)
       {
 return BadRequest(new { mensaje = "La imagen no puede superar los 5MB" });
  }

  // Crear carpeta si no existe
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "libros");
        if (!Directory.Exists(uploadsFolder))
                {
           Directory.CreateDirectory(uploadsFolder);
             }

          // Generar nombre único para el archivo
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(uploadsFolder, nombreArchivo);

    // Guardar archivo
       using (var stream = new FileStream(rutaCompleta, FileMode.Create))
       {
           await imagen.CopyToAsync(stream);
           }

                // Retornar URL relativa
       var urlImagen = $"/images/libros/{nombreArchivo}";

       return Ok(new { 
             mensaje = "Imagen subida exitosamente",
         url = urlImagen,
            nombreArchivo = nombreArchivo
              });
            }
     catch (Exception ex)
     {
      _logger.LogError(ex, "Error al subir imagen");
    return StatusCode(500, new { mensaje = "Error al subir la imagen", error = ex.Message });
            }
        }

    [HttpDelete("imagen-libro/{nombreArchivo}")]
     [Authorize]
        public IActionResult EliminarImagenLibro(string nombreArchivo)
        {
          try
    {
         var rutaCompleta = Path.Combine(_environment.WebRootPath, "images", "libros", nombreArchivo);

   if (System.IO.File.Exists(rutaCompleta))
          {
   System.IO.File.Delete(rutaCompleta);
   return Ok(new { mensaje = "Imagen eliminada exitosamente" });
      }

                return NotFound(new { mensaje = "Imagen no encontrada" });
       }
    catch (Exception ex)
            {
         _logger.LogError(ex, "Error al eliminar imagen");
                return StatusCode(500, new { mensaje = "Error al eliminar la imagen", error = ex.Message });
            }
   }
 }
}
