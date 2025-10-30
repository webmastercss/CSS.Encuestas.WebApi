using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Application.Interfaces.Services;

namespace CSS.Encuestas.Application.Services;
public class EncuestaService(IEncuestaRepository repo) : IEncuestaService
{

    readonly IEncuestaRepository _repo = repo;

    public async Task<string> CrearAsync(CrearEncuestaDto dto)
    {

        ArgumentNullException.ThrowIfNull("Error al crear la encuesta");

        return  await _repo.AddAsync(dto);
    }

    public Task<IEnumerable<PreguntaEstadisticaDto>?> Estadisticas(int id)
    {
        return id <= 0 ? throw new ArgumentOutOfRangeException() : _repo.Estadisticas(id);
    }

    public async Task<EncuestaDetalleDto?> GetAsync(string uuid)
    {
        if (!Guid.TryParse(uuid, out _))
        {
           throw new ArgumentException("El UUID proporcionado no es válido.");
        }

        return await _repo.GetAsync(uuid);
    }

    public Task<IEnumerable<EncuestaResumenDto>> GetAsync()
    {
        return _repo.GetAsync();
    }
}