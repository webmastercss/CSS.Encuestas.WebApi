using CSS.Encuestas.Application.Dtos.Ips;
using CSS.Encuestas.Application.Extensions.Mappings;
using CSS.Encuestas.Application.Extensions.Serialize;
using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace CSS.Encuestas.Infrastructure.Services;

public class IpsService(HttpClient client, IOptions<ApiIpsOptions> options) : IIpsService
{
    private readonly HttpClient _client = client;
    private readonly ApiIpsOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<IEnumerable<ConsultarIpsDto>> GetAsync()
    {
        // Agregar encabezado solo una vez
        if (!_client.DefaultRequestHeaders.Contains("X-App-Token"))
            _client.DefaultRequestHeaders.Add("X-App-Token", _options.ApiKey);

        IEnumerable<IpsResponseDtos> ips = [];

        var response = await _client.GetAsync(_options.EndPoint);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            ips = data.Deserialize<IEnumerable<IpsResponseDtos>>();
        }

        return ips.MapToEnumerable<ConsultarIpsDto>();
    }
}
