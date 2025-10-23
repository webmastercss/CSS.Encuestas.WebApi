using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Domain.Entities;

namespace CSS.Encuestas.Application.Interfaces;
public interface IEncuestaService
{
    Task<EncuestaRealizadaDto> CrearAsync(CrearEncuestaDto dto);

}
