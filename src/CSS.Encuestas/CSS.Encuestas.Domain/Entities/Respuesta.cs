using System.ComponentModel.DataAnnotations;

namespace CSS.Encuestas.Domain.Entities;
public class Respuesta
{
    public int Id { get; set; }

    public int EncuestaId { get; set; }
    public Encuesta Encuesta { get; set; } = default!;

    [MaxLength(150)]
    public string? UsuarioIdentidad { get; set; } // correo, id, etc.

    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public ICollection<RespuestaDetalle> Detalles { get; set; } = new List<RespuestaDetalle>();
}