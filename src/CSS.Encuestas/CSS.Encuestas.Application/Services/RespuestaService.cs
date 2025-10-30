using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Application.Interfaces.Services;

namespace CSS.Encuestas.Application.Services;
public class RespuestaService(IRespuestaRepository repo) : IRespuestaService
{
    private readonly IRespuestaRepository _repo = repo;

    public async Task<string?> AddAsync(string uuid, ResponderEncuestaDto dto)
    {
        if (!Guid.TryParse(uuid, out _))
        {
            throw new ArgumentException("El UUID proporcionado no es válido.");
        }

        ArgumentNullException.ThrowIfNull(dto);

        dto.Uuid = uuid;

        return await _repo.AddAsync(dto);
    }
}
