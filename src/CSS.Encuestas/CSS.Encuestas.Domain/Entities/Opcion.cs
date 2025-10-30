using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Domain.Entities;
public class Opcion
{
    public int Id { get; set; }

    public int PreguntaId { get; set; }
    public Pregunta Pregunta { get; set; } = default!;

    [MaxLength(200)]
    public string Texto { get; set; } = default!;

    public int Orden { get; set; } = 0;
}