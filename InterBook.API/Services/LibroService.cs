using InterBook.API.Data;
using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using InterBook.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InterBook.API.Services
{
    public class LibroService : ILibroService
    {
        private readonly InterBookDbContext _context;

        public LibroService(InterBookDbContext context)
        {
            _context = context;
        }

        public async Task<ListaPaginadaDto<LibroResponseDto>> ObtenerLibros(BusquedaLibroDto busqueda)
        {
            try
            {
                var query = _context.Libros
                    .Include(l => l.Propietario)
                    .Where(l => l.Activo)
                    .AsQueryable();

                // Filtrar por disponibilidad
                if (busqueda.SoloDisponibles)
                {
                    query = query.Where(l => l.EstadoDisponibilidad == "Disponible");
                }

                // Filtrar por término de búsqueda
                if (!string.IsNullOrWhiteSpace(busqueda.TerminoBusqueda))
                {
                    var termino = busqueda.TerminoBusqueda.ToLower();
                    query = query.Where(l =>
                        l.Titulo.ToLower().Contains(termino) ||
                        l.Autor.ToLower().Contains(termino) ||
                        l.ISBN.Contains(termino));
                }

                // Filtrar por categoría
                if (!string.IsNullOrWhiteSpace(busqueda.Categoria))
                {
                    query = query.Where(l => l.Categoria == busqueda.Categoria);
                }

                // Filtrar por ciudad
                if (!string.IsNullOrWhiteSpace(busqueda.Ciudad))
                {
                    query = query.Where(l => l.Ubicacion.Contains(busqueda.Ciudad));
                }

                // Contar total de elementos
                var totalElementos = await query.CountAsync();

                // Calcular total de páginas
                var totalPaginas = (int)Math.Ceiling(totalElementos / (double)busqueda.TamañoPagina);

                // Aplicar paginación
                var libros = await query
                    .OrderByDescending(l => l.FechaPublicacion)
                    .Skip((busqueda.Pagina - 1) * busqueda.TamañoPagina)
                    .Take(busqueda.TamañoPagina)
                    .Select(l => new LibroResponseDto
                    {
                        Id = l.Id,
                        UsuarioId = l.UsuarioId,
                        NombrePropietario = l.Propietario.Nombre,
                        Titulo = l.Titulo,
                        Autor = l.Autor,
                        ISBN = l.ISBN,
                        Categoria = l.Categoria,
                        AñoPublicacion = l.AñoPublicacion,
                        Descripcion = l.Descripcion,
                        EstadoFisico = l.EstadoFisico,
                        EstadoDisponibilidad = l.EstadoDisponibilidad,
                        ImagenUrl = l.ImagenUrl,
                        Ubicacion = l.Ubicacion,
                        FechaPublicacion = l.FechaPublicacion,
                        Activo = l.Activo
                    })
                    .ToListAsync();

                return new ListaPaginadaDto<LibroResponseDto>
                {
                    Elementos = libros,
                    TotalElementos = totalElementos,
                    TotalPaginas = totalPaginas,
                    PaginaActual = busqueda.Pagina
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener libros: {ex.Message}");
            }
        }

        public async Task<LibroResponseDto> ObtenerLibroPorId(int id)
        {
            try
            {
                var libro = await _context.Libros
                    .Include(l => l.Propietario)
                    .FirstOrDefaultAsync(l => l.Id == id && l.Activo);

                if (libro == null) return null;

                return new LibroResponseDto
                {
                    Id = libro.Id,
                    UsuarioId = libro.UsuarioId,
                    NombrePropietario = libro.Propietario.Nombre,
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    ISBN = libro.ISBN,
                    Categoria = libro.Categoria,
                    AñoPublicacion = libro.AñoPublicacion,
                    Editorial = libro.Editorial,
                    Descripcion = libro.Descripcion,
                    EstadoFisico = libro.EstadoFisico,
                    EstadoDisponibilidad = libro.EstadoDisponibilidad,
                    ImagenUrl = libro.ImagenUrl,
                    Ubicacion = libro.Ubicacion,
                    FechaPublicacion = libro.FechaPublicacion,
                    Activo = libro.Activo
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener libro: {ex.Message}");
            }
        }

        public async Task<LibroResponseDto> CrearLibro(LibroDto libroDto, int usuarioId)
        {
            try
            {
                var nuevoLibro = new Libro
                {
                    UsuarioId = usuarioId,
                    Titulo = libroDto.Titulo,
                    Autor = libroDto.Autor,
                    ISBN = libroDto.ISBN ?? string.Empty,
                    Categoria = libroDto.Categoria ?? string.Empty,
                    AñoPublicacion = libroDto.AñoPublicacion,
                    Editorial = libroDto.Editorial ?? string.Empty,
                    Descripcion = libroDto.Descripcion ?? string.Empty,
                    EstadoFisico = libroDto.EstadoFisico ?? "Buen Estado",
                    EstadoDisponibilidad = "Disponible",
                    ImagenUrl = libroDto.ImagenUrl ?? string.Empty,
                    Ubicacion = libroDto.Ubicacion ?? string.Empty,
                    FechaPublicacion = DateTime.UtcNow,
                    Activo = true
                };

                _context.Libros.Add(nuevoLibro);
                await _context.SaveChangesAsync();

                return await ObtenerLibroPorId(nuevoLibro.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear libro: {ex.Message}", ex);
            }
        }

        public async Task<LibroResponseDto> ActualizarLibro(int id, LibroDto libroDto)
        {
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null) return null!;

                libro.Titulo = libroDto.Titulo;
                libro.Autor = libroDto.Autor;
                libro.ISBN = libroDto.ISBN ?? libro.ISBN;
                libro.Categoria = libroDto.Categoria ?? libro.Categoria;
                libro.AñoPublicacion = libroDto.AñoPublicacion ?? libro.AñoPublicacion;
                libro.Editorial = libroDto.Editorial ?? libro.Editorial;
                libro.Descripcion = libroDto.Descripcion ?? libro.Descripcion;
                libro.EstadoFisico = libroDto.EstadoFisico ?? libro.EstadoFisico;
                libro.Ubicacion = libroDto.Ubicacion ?? libro.Ubicacion;
                
                // Solo actualizar imagen si se proporciona una nueva
                if (!string.IsNullOrWhiteSpace(libroDto.ImagenUrl))
                {
                    libro.ImagenUrl = libroDto.ImagenUrl;
                }

                await _context.SaveChangesAsync();

                return await ObtenerLibroPorId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar libro: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarLibro(int id)
        {
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null) return false;

                // Solo permitir eliminar si está disponible
                if (libro.EstadoDisponibilidad != "Disponible")
                    return false;

                libro.Activo = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> ObtenerCategorias()
        {
            try
            {
                return await _context.Libros
                    .Where(l => l.Activo && !string.IsNullOrEmpty(l.Categoria))
                    .Select(l => l.Categoria)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<List<LibroResponseDto>> ObtenerLibrosPorUsuario(int usuarioId)
        {
            try
            {
                var libros = await _context.Libros
                    .Include(l => l.Propietario)
                    .Where(l => l.UsuarioId == usuarioId && l.Activo)
                    .OrderByDescending(l => l.FechaPublicacion)
                    .Select(l => new LibroResponseDto
                    {
                        Id = l.Id,
                        UsuarioId = l.UsuarioId,
                        NombrePropietario = l.Propietario != null ? l.Propietario.Nombre : null,
                        Titulo = l.Titulo,
                        Autor = l.Autor,
                        ISBN = l.ISBN,
                        Categoria = l.Categoria,
                        AñoPublicacion = l.AñoPublicacion,
                        Editorial = l.Editorial,
                        Descripcion = l.Descripcion,
                        EstadoFisico = l.EstadoFisico,
                        EstadoDisponibilidad = l.EstadoDisponibilidad,
                        ImagenUrl = l.ImagenUrl,
                        Ubicacion = l.Ubicacion,
                        FechaPublicacion = l.FechaPublicacion,
                        Activo = l.Activo
                    })
                    .ToListAsync();

                return libros;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener libros del usuario: {ex.Message}", ex);
            }
        }
    }
}