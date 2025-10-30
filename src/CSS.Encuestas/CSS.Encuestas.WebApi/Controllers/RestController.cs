using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public abstract class RestController : ControllerBase
{

    protected ActionResult InternalServerError()
    {

        return StatusCode(StatusCodes.Status500InternalServerError);
    }


    protected ActionResult InternalServerError(object data)
    {

        return StatusCode(StatusCodes.Status500InternalServerError, data);
    }



}