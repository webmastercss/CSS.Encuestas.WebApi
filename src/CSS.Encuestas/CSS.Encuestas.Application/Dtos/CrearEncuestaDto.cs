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
    public string? Uuid { get; set; }
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

public class EncuestaDetalleDto
{
    public int Id { get; set; }
    public string Uuid { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public string? Descripcion { get; set; }

    public List<PreguntaDetalleDto> Preguntas { get; set; } = new();
}

/// <summary>
/// Representa una pregunta dentro de una encuesta.
/// </summary>
public class PreguntaDetalleDto
{
    public int Id { get; set; }
    public string Texto { get; set; } = default!;
    public string Tipo { get; set; } = default!; // Ejemplo: "TextoCorto", "OpcionUnica"
    public bool EsObligatoria { get; set; }
    public int Orden { get; set; }
    public int? EscalaMin { get; set; }
    public int? EscalaMax { get; set; }

    public List<OpcionDto> Opciones { get; set; } = new();
}

/// <summary>
/// Representa una opción de respuesta asociada a una pregunta.
/// </summary>
public class OpcionDto
{
    public int Id { get; set; }
    public string Texto { get; set; } = default!;
    public int Orden { get; set; }
}



public class EncuestaResumenDto
{
    public int Id { get; set; }
    public string Uuid { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class PreguntaEstadisticaDto
{

    public int Id { get; set; }


    public string Texto { get; set; } = default!;


    public string Tipo { get; set; } = default!;


    public int TotalRegistros { get; set; }


    public List<OpcionConteoDto>? Opciones { get; set; }


    public double? Promedio { get; set; }


    public int? Min { get; set; }


    public int? Max { get; set; }
}


public class OpcionConteoDto
{

    public int Id { get; set; }


    public string Texto { get; set; } = default!;


    public int Conteo { get; set; }
}