using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Extensions.Mappings;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Domain.Entities;

namespace CSS.Encuestas.Application.Services;
public class EncuestaService(IEncuestaRepository repo) : IEncuestaService
{
    public async Task<EncuestaRealizadaDto> CrearAsync(CrearEncuestaDto dto)
    {

        try
        {

            var entity =  dto.MapTo<EncuestaPreanalitica>();
            return new EncuestaRealizadaDto
            {
                Id = await repo.AddAsync(entity),
                Mensaje = "Encuesta respondida exitosamente"
            };

        }
        catch (Exception)
        {
            throw;
        }
    }
}