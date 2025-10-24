using CSS.Encuestas.Application.Dtos.Encuesta;
using CSS.Encuestas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;

public class EncuestaPreanaliticaController(IEncuestaService service, ILogger<EncuestaPreanaliticaController> logger) : RestController
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
            return InternalServerError("Error al agregar la Encuesta");
        }
    }
}