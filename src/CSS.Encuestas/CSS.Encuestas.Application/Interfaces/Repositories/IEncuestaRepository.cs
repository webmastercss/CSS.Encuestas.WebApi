using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Domain.Entities;

namespace CSS.Encuestas.Application.Interfaces.Repositories;
public interface IEncuestaRepository
{
    Task<string> AddAsync(CrearEncuestaDto dto);
    Task<EncuestaDetalleDto?> GetAsync(string uuid);
    Task<IEnumerable<EncuestaResumenDto>> GetAsync();
    Task<IEnumerable<PreguntaEstadisticaDto>?> Estadisticas(int id);

}
