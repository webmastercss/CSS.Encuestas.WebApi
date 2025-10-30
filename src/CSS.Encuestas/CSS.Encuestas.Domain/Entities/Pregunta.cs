using CSS.Encuestas.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Domain.Entities;
public class Pregunta
{
    public int Id { get; set; }

    public int EncuestaId { get; set; }
    public Encuesta Encuesta { get; set; } = default!;

    [MaxLength(300)]
    public string Texto { get; set; } = default!;

    public TipoPregunta Tipo { get; set; }

    public bool EsObligatoria { get; set; } = true;

    // Para Escala (opcional)
    public int? EscalaMin { get; set; }  // p.ej. 1
    public int? EscalaMax { get; set; }  // p.ej. 5

    public int Orden { get; set; } = 0;

    public ICollection<Opcion> Opciones { get; set; } = new List<Opcion>();
}
