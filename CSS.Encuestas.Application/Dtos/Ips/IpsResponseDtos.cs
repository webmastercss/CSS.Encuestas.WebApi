using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSS.Encuestas.Application.Dtos.Ips;
public class IpsResponseDtos
{
    [JsonPropertyName("nom_sede_ips")]
    public string Nombre { get; set; }
    [JsonPropertyName("naturaleza")]
    public string Naturaleza { get; set; }
    [JsonPropertyName("nom_grupo_capacidad")]
    public string NombreGrupoCapacidad { get; set; }
    [JsonPropertyName("num_cantidad_capacidad_instalada")]
    public string CapacidadInstalada { get; set; } 

}

