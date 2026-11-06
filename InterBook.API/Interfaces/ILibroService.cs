using System.Collections.Generic;
using System.Threading.Tasks;
using InterBook.API.DTOs;

namespace InterBook.API.Interfaces
{
    public interface ILibroService
    {
        Task<ListaPaginadaDto<LibroResponseDto>> ObtenerLibros(BusquedaLibroDto busqueda);
        Task<LibroResponseDto> ObtenerLibroPorId(int id);
        Task<LibroResponseDto> CrearLibro(LibroDto libroDto, int usuarioId);
        Task<LibroResponseDto> ActualizarLibro(int id, LibroDto libroDto);
        Task<bool> EliminarLibro(int id);
        Task<List<string>> ObtenerCategorias();
        Task<List<LibroResponseDto>> ObtenerLibrosPorUsuario(int usuarioId);
    }
}