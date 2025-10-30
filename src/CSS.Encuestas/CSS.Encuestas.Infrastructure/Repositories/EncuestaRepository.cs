using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Domain.Entities;
using CSS.Encuestas.Domain.Enums;
using CSS.Encuestas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.Infrastructure.Repositories;
public class EncuestaRepository(EncuestasDbContext db) : IEncuestaRepository
{
    private readonly EncuestasDbContext _db = db;

    public async Task<string> AddAsync(CrearEncuestaDto dto)
    {

        var enc = new Encuesta
        {
            Titulo = dto.Titulo,
            Uuid = Guid.NewGuid().ToString(),
            Descripcion = dto.Descripcion,
            Preguntas = dto.Preguntas.Select(p => new Pregunta
            {
                Texto = p.Texto,
                Tipo = p.Tipo,
                EsObligatoria = p.EsObligatoria,
                Orden = p.Orden,
                EscalaMin = p.EscalaMin,
                EscalaMax = p.EscalaMax,
                Opciones = (p.Opciones ?? new()).Select(o => new Opcion
                {
                    Texto = o.Texto,
                    Orden = o.Orden
                }).ToList()
            }).ToList()
        };

        _db.Encuestas.Add(enc);
        await _db.SaveChangesAsync();

        return enc.Uuid;
    }

    public async Task<EncuestaDetalleDto?> GetAsync(string uuid)
    {

        var encuesta = await _db.Encuestas
         .Include(e => e.Preguntas.OrderBy(p => p.Orden))
             .ThenInclude(p => p.Opciones.OrderBy(o => o.Orden))
         .FirstOrDefaultAsync(e => e.Uuid == uuid);

        if (encuesta is null) return null;


        var dto = new EncuestaDetalleDto
        {
            Id = encuesta.Id,
            Uuid = encuesta.Uuid,
            Titulo = encuesta.Titulo,
            Descripcion = encuesta.Descripcion,
            Preguntas = encuesta.Preguntas
            .OrderBy(p => p.Orden)
            .Select(p => new PreguntaDetalleDto
            {
                Id = p.Id,
                Texto = p.Texto,
                Tipo = p.Tipo.ToString(),
                EsObligatoria = p.EsObligatoria,
                Orden = p.Orden,
                EscalaMin = p.EscalaMin,
                EscalaMax = p.EscalaMax,
                Opciones = p.Opciones
                    .OrderBy(o => o.Orden)
                    .Select(o => new OpcionDto
                    {
                        Id = o.Id,
                        Texto = o.Texto,
                        Orden = o.Orden
                    })
                    .ToList()
            })
            .ToList()
        };

        return dto;

    }

    public async Task<IEnumerable<EncuestaResumenDto>> GetAsync()
    {
        var data = await _db.Encuestas
            .Select(e => new EncuestaResumenDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Uuid = e.Uuid,
                Descripcion = e.Descripcion,
                FechaCreacion = e.FechaCreacion
            })
            .ToListAsync();

        return data;
    }


    public async Task<IEnumerable<PreguntaEstadisticaDto>?> Estadisticas(int id)
    {
        var encuestaExiste = await _db.Encuestas.AnyAsync(e => e.Id == id);

        if (!encuestaExiste) return null;

        var preguntas = await _db.Preguntas
            .Where(p => p.EncuestaId == id)
            .Include(p => p.Opciones)
            .OrderBy(p => p.Orden)
            .ToListAsync();

        var resultados = new List<PreguntaEstadisticaDto>();

        foreach (var p in preguntas)
        {
            var total = await _db.RespuestasDetalle.CountAsync(d => d.PreguntaId == p.Id);

            if (p.Tipo == TipoPregunta.OpcionUnica || p.Tipo == TipoPregunta.OpcionMultiple)
            {
                var conteos = await _db.RespuestasDetalle
                    .Where(d => d.PreguntaId == p.Id && d.OpcionId != null)
                    .GroupBy(d => d.OpcionId)
                    .Select(g => new { OpcionId = g.Key!.Value, Conteo = g.Count() })
                    .ToListAsync();

                resultados.Add(new PreguntaEstadisticaDto
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = total,
                    Opciones = p.Opciones.Select(o => new OpcionConteoDto
                    {
                        Id = o.Id,
                        Texto = o.Texto,
                        Conteo = conteos.FirstOrDefault(c => c.OpcionId == o.Id)?.Conteo ?? 0
                    })
                    .OrderBy(o => o.Id)
                    .ToList()
                });
            }
            else if (p.Tipo == TipoPregunta.Escala)
            {
                var stats = await _db.RespuestasDetalle
                    .Where(d => d.PreguntaId == p.Id && d.ValorEntero != null)
                    .Select(d => d.ValorEntero!.Value)
                    .ToListAsync();

                resultados.Add(new PreguntaEstadisticaDto
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = stats.Count,
                    Promedio = stats.Count == 0 ? 0 : Math.Round(stats.Average(), 2),
                    Min = stats.Count == 0 ? 0 : stats.Min(),
                    Max = stats.Count == 0 ? 0 : stats.Max()
                });
            }
            else
            {
                resultados.Add(new PreguntaEstadisticaDto
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = total
                });
            }
        }

        return resultados;
    }


}
