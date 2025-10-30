using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Domain.Entities;
using CSS.Encuestas.Domain.Enums;
using CSS.Encuestas.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.WebApi.Controllers;

public class EncuestasController: RestController
{

    private readonly EncuestasDbContext _db;

    public EncuestasController(EncuestasDbContext db) => _db = db;

    // GET: api/encuestas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Listar()
    {
        var data = await _db.Encuestas
            .Select(e => new { e.Id, e.Titulo, e.Uuid, e.Descripcion, e.FechaCreacion })
            .ToListAsync();

        return Ok(data);
    }

    // GET: api/encuestas/5 (incluye preguntas y opciones)
    [HttpGet("{uuid}")]
    public async Task<ActionResult<object>> Obtener(string uuid)
    {
        var encuesta = await _db.Encuestas
            .Include(e => e.Preguntas.OrderBy(p => p.Orden))
                .ThenInclude(p => p.Opciones.OrderBy(o => o.Orden))
            .FirstOrDefaultAsync(e => e.Uuid == uuid);

        if (encuesta is null) return NotFound();

        return Ok(new
        {
            encuesta.Id,
            encuesta.Uuid,
            encuesta.Titulo,
            encuesta.Descripcion,
            Preguntas = encuesta.Preguntas.Select(p => new
            {
                p.Id,
                p.Texto,
                Tipo = p.Tipo.ToString(),
                p.EsObligatoria,
                p.Orden,
                p.EscalaMin,
                p.EscalaMax,
                Opciones = p.Opciones.Select(o => new { o.Id, o.Texto, o.Orden })
            })
        });
    }

    // POST: api/encuestas  (crear encuesta)
    [HttpPost]
    public async Task<ActionResult> Crear([FromBody] CrearEncuestaDto dto)
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

        return CreatedAtAction(nameof(Obtener), new { id = enc.Id }, new { enc.Id });
    }

    // POST: api/encuestas/{id}/respuestas (enviar respuestas)
    [HttpPost("{uuid}/respuestas")]
    public async Task<ActionResult> Responder(string uuid, [FromBody] ResponderEncuestaDto dto)
    {
        var encuesta = await _db.Encuestas
            .Include(e => e.Preguntas)
                .ThenInclude(p => p.Opciones)
            .FirstOrDefaultAsync(e => e.Uuid == uuid);

        if (encuesta is null) return NotFound("Encuesta no existe.");

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
                return BadRequest($"La pregunta {pregunta.Id} es obligatoria.");
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
                    if (r.ValorEntero is null) return BadRequest("Falta valor de escala.");
                    if (pregunta.EscalaMin.HasValue && r.ValorEntero < pregunta.EscalaMin ||
                        pregunta.EscalaMax.HasValue && r.ValorEntero > pregunta.EscalaMax)
                        return BadRequest("Valor de escala fuera de rango.");
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
                            return BadRequest($"La opción {opId} no pertenece a la pregunta {pregunta.Id}.");

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
        return Ok(new { respuesta.Id });
    }

    // GET: api/encuestas/{id}/estadisticas
    [HttpGet("{id:int}/estadisticas")]
    public async Task<ActionResult> Estadisticas(int id)
    {
        var encuestaExiste = await _db.Encuestas.AnyAsync(e => e.Id == id);
        if (!encuestaExiste) return NotFound();

        var preguntas = await _db.Preguntas
            .Where(p => p.EncuestaId == id)
            .Include(p => p.Opciones)
            .OrderBy(p => p.Orden)
            .ToListAsync();

        var resultados = new List<object>();

        foreach (var p in preguntas)
        {
            // total de respuestas que tienen detalle para esta pregunta
            var total = await _db.RespuestasDetalle.CountAsync(d => d.PreguntaId == p.Id);

            if (p.Tipo == TipoPregunta.OpcionUnica || p.Tipo == TipoPregunta.OpcionMultiple)
            {
                var conteos = await _db.RespuestasDetalle
                    .Where(d => d.PreguntaId == p.Id && d.OpcionId != null)
                    .GroupBy(d => d.OpcionId)
                    .Select(g => new { OpcionId = g.Key!.Value, Conteo = g.Count() })
                    .ToListAsync();

                resultados.Add(new
                {
                    p.Id,
                    p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = total,
                    Opciones = p.Opciones
                        .Select(o => new
                        {
                            o.Id,
                            o.Texto,
                            Conteo = conteos.FirstOrDefault(c => c.OpcionId == o.Id)?.Conteo ?? 0
                        })
                        .OrderBy(o => o.Id)
                });
            }
            else if (p.Tipo == TipoPregunta.Escala)
            {
                var stats = await _db.RespuestasDetalle
                    .Where(d => d.PreguntaId == p.Id && d.ValorEntero != null)
                    .Select(d => d.ValorEntero!.Value)
                    .ToListAsync();

                resultados.Add(new
                {
                    p.Id,
                    p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = stats.Count,
                    Promedio = stats.Count == 0 ? 0 : Math.Round(stats.Average(), 2),
                    Min = stats.Count == 0 ? 0 : stats.Min(),  
                    Max = stats.Count == 0 ? 0 : stats.Max()   
                });
            }
            else
            {
                // Texto: solo total
                resultados.Add(new
                {
                    p.Id,
                    p.Texto,
                    Tipo = p.Tipo.ToString(),
                    TotalRegistros = total
                });
            }
        }

        return Ok(resultados);
    }

}
