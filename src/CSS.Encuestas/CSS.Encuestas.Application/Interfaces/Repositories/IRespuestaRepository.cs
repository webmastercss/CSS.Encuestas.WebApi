

using CSS.Encuestas.Application.Dtos;

namespace CSS.Encuestas.Application.Interfaces.Repositories;
public interface IRespuestaRepository
{
    Task<string?> AddAsync(ResponderEncuestaDto dto);

}
