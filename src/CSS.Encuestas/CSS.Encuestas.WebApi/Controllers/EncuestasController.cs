using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;

public class EncuestasController(IEncuestaService encuestaService, IRespuestaService respuestaService) : RestController
{

    private readonly IEncuestaService _encuestaService = encuestaService;
    private readonly IRespuestaService _respuestaService = respuestaService;

    [HttpGet]
    public async Task<ActionResult> Listar()
    {
        return Ok(await _encuestaService.GetAsync());
    }

    [HttpGet("{uuid}")]
    public async Task<ActionResult> Obtener(string uuid)
    {

        return await _encuestaService.GetAsync(uuid) is EncuestaDetalleDto dto
            ? Ok(dto)
            : NotFound();

    }

    [HttpPost]
    public async Task<ActionResult> Crear([FromBody] CrearEncuestaDto dto)
    {

        var encuesta = await _encuestaService.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { uuid = encuesta }, new { encuesta });
    }

    [HttpPost("{uuid}/respuestas")]
    public async Task<ActionResult> Responder(string uuid, ResponderEncuestaDto dto)
    {
        var respuesta = await _respuestaService.AddAsync(uuid, dto);

        return Ok(new { respuesta });
    }

    [HttpGet("{id}/estadisticas")]
    public async Task<ActionResult> Estadisticas(int id)
    {   
        return await _encuestaService.Estadisticas(id) is IEnumerable<PreguntaEstadisticaDto> estadisticas
            ? Ok(estadisticas)
            : NotFound();
    }
}
