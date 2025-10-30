namespace CSS.Encuestas.Application.Dtos.Encuesta;

using CSS.Encuestas.Application.Attributes;
using System.ComponentModel.DataAnnotations;

public class CrearEncuestaPreanaliticaDto
{

    [Required, StringLength(50, ErrorMessage = "El número de recepción no puede superar los 50 caracteres.")]
    [Sanitize]
    public string NumeroRecepcion { get; set; }

    [Required, StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
    [Sanitize]
    public string NombreCompleto { get; set; }

    [Required, StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
    [Sanitize]
    public string Identificacion { get; set; }

    [Range(0, 120, ErrorMessage = "La edad debe estar entre 0 y 120 años.")]
    public int Edad { get; set; }

    [Required, RegularExpression("^(M|F|Otro)$", ErrorMessage = "El sexo debe ser M, F u Otro.")]
    [Sanitize]
    public string Sexo { get; set; }

    // Si quieres validar celular colombiano que empiece en 3 y tenga 10 dígitos:
    [Required, RegularExpression(@"^3\d{9}$", ErrorMessage = "Debe ingresar un celular colombiano válido (10 dígitos iniciando en 3).")]
    [Sanitize]
    public string Telefono { get; set; }

    [Required, RegularExpression("^(Sí|No)$", ErrorMessage = "Embarazada debe ser 'Sí' o 'No'.")]
    [Sanitize]
    public string Embarazada { get; set; }

    [Required]
    [Sanitize]
    public string PeriodoMenstrual { get; set; }

    [Required, RegularExpression("^(Sí|No)$", ErrorMessage = "Ayuno debe ser 'Sí' o 'No'.")]
    [Sanitize]
    public string Ayuno { get; set; }

    [Required]
    [Sanitize]
    public string SintomasRespiratorios { get; set; }

    [Required]
    [Sanitize]
    public string Rechazo { get; set; }

    [Required]
    [Sanitize]
    public string MotivoRechazo { get; set; }

    [Required]
    [Sanitize]
    public string EnfermedadesBase { get; set; }

    [Required]
    [Sanitize]
    public string Medicamentos { get; set; }

    [Required]
    [Sanitize]
    public string ResponsableToma { get; set; }

    [Required]
    [Sanitize]
    public string HoraAtencion { get; set; }

    [Required, RegularExpression("^(Sí|No)$", ErrorMessage = "Consentimiento debe ser 'Sí' o 'No'.")]
    [Sanitize]
    public string Consentimiento { get; set; }

    [Required]
    [Sanitize]
    public string PacienteNombreFirma { get; set; }

    [Required]
    [Sanitize]
    public string PacienteDocFirma { get; set; }

    [Required]
    [Sanitize]
    public string RepNombre { get; set; }

    [Required]
    [Sanitize]
    public string RepDoc { get; set; }

    [Required]
    [Sanitize]
    public string FirmaPac { get; set; }

    [Required]
    [Sanitize]
    public string FirmaRep { get; set; }
}


