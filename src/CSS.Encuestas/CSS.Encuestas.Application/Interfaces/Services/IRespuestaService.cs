using CSS.Encuestas.Application.Dtos;

namespace CSS.Encuestas.Application.Interfaces.Services;
public interface IRespuestaService
{
    Task<string?> AddAsync(string uuid, ResponderEncuestaDto dto);
}
