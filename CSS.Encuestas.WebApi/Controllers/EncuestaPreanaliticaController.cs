using CSS.Encuestas.Application.Dtos;
using CSS.Encuestas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncuestaPreanaliticaController(IEncuestaService service, ILogger<EncuestaPreanaliticaController> logger) : ControllerBase
{
    // POST: /api/EncuestaPreanalitica
    [HttpPost]
    public async Task<IActionResult> Post(CrearEncuestaDto dto)
    {
        try
        {
            var respuesta = await service.CrearAsync(dto);
            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en POST /api/EncuestaPreanalitica");
            return StatusCode(500, "Error al agregar la Encuesta");
        }
    }
}