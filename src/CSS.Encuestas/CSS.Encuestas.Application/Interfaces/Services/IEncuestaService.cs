using CSS.Encuestas.Application.Dtos.Encuesta;

namespace CSS.Encuestas.Application.Interfaces.Services;
public interface IEncuestaService
{
    Task<EncuestaPreanaliticaRealizadaDto> CrearAsync(CrearEncuestaPreanaliticaDto dto);
    Task<EncuestaPreanaliticaDto?> GetAsync(int id);
}
