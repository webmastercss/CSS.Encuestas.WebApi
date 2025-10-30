using CSS.Encuestas.Application.Dtos.Ips;

namespace CSS.Encuestas.Application.Interfaces.Services;
public interface IIpsService
{
    Task<IEnumerable<ConsultarIpsDto>> GetAsync();
}
