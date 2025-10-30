using CSS.Encuestas.Application.Dtos.Encuesta;
using CSS.Encuestas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;

public class EncuestaPreanaliticaController(IEncuestaService service, ILogger<EncuestaPreanaliticaController> logger) : RestController
{
    // POST: /api/EncuestaPreanalitica
    [HttpPost]
    public async Task<IActionResult> Post(CrearEncuestaPreanaliticaDto dto)
    {
        try
        {
            var respuesta = await service.CrearAsync(dto);

            return CreatedAtAction(
                nameof(Get),                  
                new { id = respuesta.Id },    
                respuesta                     // Cuerpo opcional de la respuesta (puedes pasar solo el id si prefieres)
            );


        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en POST /api/EncuestaPreanalitica");
            return InternalServerError("Error al agregar la Encuesta");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var respuesta = await service.GetAsync(id);

            if (respuesta is null)
            {

                return NotFound();
            }

            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en POST /api/EncuestaPreanalitica");
            return InternalServerError("Error al agregar la Encuesta");
        }
    }
}