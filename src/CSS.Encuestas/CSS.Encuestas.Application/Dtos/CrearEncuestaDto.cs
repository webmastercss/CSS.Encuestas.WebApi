using CSS.Encuestas.Domain.Enums;

namespace CSS.Encuestas.Application.Dtos;
// crear/actualizar encuesta
public class CrearEncuestaDto
{
    public string Titulo { get; set; } = default!;
    public string? Descripcion { get; set; }
    public List<CrearPreguntaDto> Preguntas { get; set; } = new();
}

public class CrearPreguntaDto
{
    public string Texto { get; set; } = default!;
    public TipoPregunta Tipo { get; set; }
    public bool EsObligatoria { get; set; } = true;
    public int Orden { get; set; } = 0;

    // Escala (opcional)
    public int? EscalaMin { get; set; }
    public int? EscalaMax { get; set; }

    public List<CrearOpcionDto>? Opciones { get; set; }
}

public class CrearOpcionDto
{
    public string Texto { get; set; } = default!;
    public int Orden { get; set; } = 0;
}

// responder encuesta
public class ResponderEncuestaDto
{
    public string? UsuarioIdentidad { get; set; }
    public List<RespuestaPreguntaDto> Respuestas { get; set; } = new();
}

public class RespuestaPreguntaDto
{
    public int PreguntaId { get; set; }

    // Para abiertas/escala
    public string? ValorTexto { get; set; }
    public int? ValorEntero { get; set; }

    // Para opciones: enviar lista de opciones seleccionadas (múltiple o única)
    public List<int>? OpcionesSeleccionadas { get; set; }
}