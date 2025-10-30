namespace CSS.Encuestas.Domain.Entities;
public class EncuestaPreanalitica
{

    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string NumeroRecepcion { get; set; }
    public string NombreCompleto { get; set; }
    public string Identificacion { get; set; }
    public int Edad { get; set; }
    public string Sexo { get; set; }
    public string Telefono { get; set; }
    public string Embarazada { get; set; }
    public string PeriodoMenstrual { get; set; }
    public string Ayuno { get; set; }
    public string SintomasRespiratorios { get; set; }
    public string Rechazo { get; set; }
    public string MotivoRechazo { get; set; }
    public string EnfermedadesBase { get; set; }
    public string Medicamentos { get; set; }
    public string ResponsableToma { get; set; }
    public string HoraAtencion { get; set; }
    public string Consentimiento { get; set; }
    public string PacienteNombreFirma { get; set; }
    public string PacienteDocFirma { get; set; }
    public string RepNombre { get; set; }
    public string RepDoc { get; set; }
    public string FirmaPac { get; set; }
    public string FirmaRep { get; set; }
    public DateTime CreadoAt { get; set; } = DateTime.UtcNow;
}
