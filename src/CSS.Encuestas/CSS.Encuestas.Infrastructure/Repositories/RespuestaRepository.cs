using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Domain.Entities;
using CSS.Encuestas.Domain.Enums;
using CSS.Encuestas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.Infrastructure.Repositories;
public class RespuestaRepository(EncuestasDbContext db) : IRespuestaRepository
{

    private readonly EncuestasDbContext _db = db;

    public async Task<string?> AddAsync(ResponderEncuestaDto dto)
    {
        var encuesta = await _db.Encuestas
      .Include(e => e.Preguntas)
          .ThenInclude(p => p.Opciones)
      .FirstOrDefaultAsync(e => e.Uuid == dto.Uuid);

        if (encuesta is null) return null;

        var respuesta = new Respuesta
        {
            EncuestaId = encuesta.Id,
            UsuarioIdentidad = dto.UsuarioIdentidad
        };

        foreach (var r in dto.Respuestas)
        {
            var pregunta = encuesta.Preguntas.FirstOrDefault(p => p.Id == r.PreguntaId);
            if (pregunta is null) continue;

            // Validación simple de obligatoriedad
            if (pregunta.EsObligatoria &&
                string.IsNullOrWhiteSpace(r.ValorTexto) &&
                 r.ValorEntero is null &&
                 (r.OpcionesSeleccionadas is null || !r.OpcionesSeleccionadas.Any()))
            {
                return null;
            }

            // Manejo por tipo
            switch (pregunta.Tipo)
            {
                case TipoPregunta.TextoCorto:
                case TipoPregunta.TextoLargo:
                    respuesta.Detalles.Add(new RespuestaDetalle
                    {
                        PreguntaId = pregunta.Id,
                        ValorTexto = r.ValorTexto
                    });
                    break;

                case TipoPregunta.Escala:
                    if (r.ValorEntero is null) return null;
                    if (pregunta.EscalaMin.HasValue && r.ValorEntero < pregunta.EscalaMin ||
                        pregunta.EscalaMax.HasValue && r.ValorEntero > pregunta.EscalaMax)
                        return  null;
                    respuesta.Detalles.Add(new RespuestaDetalle
                    {
                        PreguntaId = pregunta.Id,
                        ValorEntero = r.ValorEntero
                    });
                    break;

                case TipoPregunta.OpcionUnica:
                case TipoPregunta.OpcionMultiple:
                    var seleccion = r.OpcionesSeleccionadas ?? new();
                    foreach (var opId in seleccion)
                    {
                        if (!pregunta.Opciones.Any(o => o.Id == opId))
                            return null;

                        respuesta.Detalles.Add(new RespuestaDetalle
                        {
                            PreguntaId = pregunta.Id,
                            OpcionId = opId
                        });
                    }
                    break;
            }
        }

        _db.Respuestas.Add(respuesta);
        await _db.SaveChangesAsync();

        return respuesta.Id.ToString();
    }
}
