using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Application.Dtos;
public record EnviarCorreoDto
{
    [EmailAddress]
    public string Correo { get; init; }
    public string? Asunto { get; init; }

    public string? Cuerpo { get; init; }

}
