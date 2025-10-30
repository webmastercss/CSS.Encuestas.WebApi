using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Domain.Entities;
public class Encuesta
{
    public int Id { get; set; }

    public string Uuid { get; set; }

    [MaxLength(200)]
    public string Titulo { get; set; } = default!;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Pregunta> Preguntas { get; set; } = new List<Pregunta>();
}