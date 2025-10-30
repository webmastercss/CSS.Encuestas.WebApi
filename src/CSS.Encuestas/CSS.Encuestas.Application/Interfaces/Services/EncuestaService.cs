using CSS.Encuestas.Application.Dtos;

namespace CSS.Encuestas.Application.Interfaces.Services;
public interface IEncuestaService
{    Task<string> CrearAsync(CrearEncuestaDto dto);
    Task<EncuestaDetalleDto?> GetAsync(string uuid);

    Task<IEnumerable<EncuestaResumenDto>> GetAsync();

    Task<IEnumerable<PreguntaEstadisticaDto>?> Estadisticas(int id);
}
