using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Application.Dtos;
public record EnviarCorreoDto
{
    
    public required string[] Destinatarios { get; init; }
    public string? Asunto { get; init; }
    public string? Cuerpo { get; init; }

}
