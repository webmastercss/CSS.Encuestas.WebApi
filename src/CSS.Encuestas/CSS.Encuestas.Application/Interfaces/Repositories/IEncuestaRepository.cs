using CSS.Encuestas.Application.Dtos.Encuesta;
using CSS.Encuestas.Domain.Entities;

namespace CSS.Encuestas.Application.Interfaces.Repositories;
public interface IEncuestaRepository
{
    Task<int> AddAsync(EncuestaPreanalitica entity);
    Task<EncuestaPreanaliticaDto?> GetAsync(int id);

}
