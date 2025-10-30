using CSS.Encuestas.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Encuestas.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IpsController(IIpsService ipsService) : RestController
{
    private readonly IIpsService _ipsService = ipsService;

    [HttpGet]
    public async Task<ActionResult> Get() {

        try
        {
            return Ok(await _ipsService.GetAsync());
        }
        catch(Exception e) {

            return InternalServerError(e.Message);
        }
    
    }
}
