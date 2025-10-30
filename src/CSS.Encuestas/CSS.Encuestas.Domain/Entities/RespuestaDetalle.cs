namespace CSS.Encuestas.Domain.Entities;
public class RespuestaDetalle
{
    public int Id { get; set; }

    public int RespuestaId { get; set; }
    public Respuesta Respuesta { get; set; } = default!;

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = default!;

    // Para preguntas abiertas o escala
    public string? ValorTexto { get; set; }     // TextoCorto/TextoLargo
    public int? ValorEntero { get; set; }       // Escala, o mapear opción por Id

    // Para opciones (única o múltiple) guardamos Id de opción seleccionada (una por fila).
    public int? OpcionId { get; set; }
    public Opcion? Opcion { get; set; }
}