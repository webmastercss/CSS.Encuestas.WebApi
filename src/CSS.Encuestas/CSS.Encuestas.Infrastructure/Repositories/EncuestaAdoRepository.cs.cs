using CSS.Encuestas.Application.Dtos.Encuesta;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CSS.Encuestas.Infrastructure.Repositories;
public class EncuestaAdoRepository : IEncuestaRepository
{
    private readonly string _connectionString;

    // 🟢 Inyectamos IConfiguration para leer la cadena de conexión del appsettings.json
    public EncuestaAdoRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("local")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'local' en appsettings.json.");
    }

    public async Task<int> AddAsync(EncuestaPreanalitica e)
    {
        const string sql = @"INSERT INTO dbo.EncuestaPreanalitica (
                              Fecha, NumeroRecepcion, NombreCompleto, Identificacion, Edad, Sexo, Telefono,
                              Embarazada, PeriodoMenstrual, Ayuno, SintomasRespiratorios, Rechazo, MotivoRechazo,
                              EnfermedadesBase, Medicamentos, ResponsableToma, HoraAtencion, Consentimiento,
                              PacienteNombreFirma, PacienteDocFirma, RepNombre, RepDoc, FirmaPac, FirmaRep, CreadoAt
                            )

                            VALUES (
                              @Fecha, @NumeroRecepcion, @NombreCompleto, @Identificacion, @Edad, @Sexo, @Telefono,
                              @Embarazada, @PeriodoMenstrual, @Ayuno, @SintomasRespiratorios, @Rechazo, @MotivoRechazo,
                              @EnfermedadesBase, @Medicamentos, @ResponsableToma, @HoraAtencion, @Consentimiento,
                              @PacienteNombreFirma, @PacienteDocFirma, @RepNombre, @RepDoc, @FirmaPac, @FirmaRep, SYSUTCDATETIME()
                            );
                            
                            SELECT SCOPE_IDENTITY();";

        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync();
        using var cmd = new SqlCommand(sql, con);


        cmd.Parameters.AddWithValue("@Fecha", e.Fecha.Date);
        cmd.Parameters.AddWithValue("@NumeroRecepcion", e.NumeroRecepcion);
        cmd.Parameters.AddWithValue("@NombreCompleto", e.NombreCompleto);
        cmd.Parameters.AddWithValue("@Identificacion", e.Identificacion);
        cmd.Parameters.AddWithValue("@Edad", e.Edad);
        cmd.Parameters.AddWithValue("@Sexo", e.Sexo);
        cmd.Parameters.AddWithValue("@Telefono", e.Telefono);
        cmd.Parameters.AddWithValue("@Embarazada", e.Embarazada);
        cmd.Parameters.AddWithValue("@PeriodoMenstrual", e.PeriodoMenstrual);
        cmd.Parameters.AddWithValue("@Ayuno", e.Ayuno);
        cmd.Parameters.AddWithValue("@SintomasRespiratorios", e.SintomasRespiratorios);
        cmd.Parameters.AddWithValue("@Rechazo", e.Rechazo);
        cmd.Parameters.AddWithValue("@MotivoRechazo", e.MotivoRechazo);
        cmd.Parameters.AddWithValue("@EnfermedadesBase", e.EnfermedadesBase);
        cmd.Parameters.AddWithValue("@Medicamentos", e.Medicamentos);
        cmd.Parameters.AddWithValue("@ResponsableToma", e.ResponsableToma);
        cmd.Parameters.AddWithValue("@HoraAtencion", e.HoraAtencion);
        cmd.Parameters.AddWithValue("@Consentimiento", e.Consentimiento);
        cmd.Parameters.AddWithValue("@PacienteNombreFirma", e.PacienteNombreFirma);
        cmd.Parameters.AddWithValue("@PacienteDocFirma", e.PacienteDocFirma);
        cmd.Parameters.AddWithValue("@RepNombre", e.RepNombre);
        cmd.Parameters.AddWithValue("@RepDoc", e.RepDoc);
        cmd.Parameters.AddWithValue("@FirmaPac", e.FirmaPac);
        cmd.Parameters.AddWithValue("@FirmaRep", e.FirmaRep);

        return  Convert.ToInt32(cmd.ExecuteScalar());


    }

    public async Task<EncuestaPreanaliticaDto?> GetAsync(int id)
    {
        const string sql = @"SELECT 
                              Fecha, NumeroRecepcion, NombreCompleto, Identificacion, Edad, Sexo, Telefono,
                              Embarazada, PeriodoMenstrual, Ayuno, SintomasRespiratorios, Rechazo, MotivoRechazo,
                              EnfermedadesBase, Medicamentos, ResponsableToma, HoraAtencion, Consentimiento,
                              PacienteNombreFirma, PacienteDocFirma, RepNombre, RepDoc, FirmaPac, FirmaRep, CreadoAt
                              FROM dbo.EncuestaPreanalitica";

        await using var con = new SqlConnection(_connectionString);

        await con.OpenAsync();
        using var cmd = new SqlCommand(sql, con);

        var reader = cmd.ExecuteReader();

        if (reader.Read()) {


           return new EncuestaPreanaliticaDto
           {
                Id = id,
                NumeroRecepcion = reader["NumeroRecepcion"]?.ToString(),
                NombreCompleto = reader["NombreCompleto"]?.ToString(),
                Identificacion = reader["Identificacion"]?.ToString(),
                Edad = reader.GetInt32(reader.GetOrdinal("Edad")),
                Sexo = reader["Sexo"]?.ToString(),
                Telefono = reader["Telefono"]?.ToString(),
                Embarazada = reader["Embarazada"]?.ToString(),
                PeriodoMenstrual = reader["PeriodoMenstrual"]?.ToString(),
                Ayuno = reader["Ayuno"]?.ToString(),
                SintomasRespiratorios = reader["SintomasRespiratorios"]?.ToString(),
                Rechazo = reader["Rechazo"]?.ToString(),
                MotivoRechazo = reader["MotivoRechazo"]?.ToString(),
                EnfermedadesBase = reader["EnfermedadesBase"]?.ToString(),
                Medicamentos = reader["Medicamentos"]?.ToString(),
                ResponsableToma = reader["ResponsableToma"]?.ToString(),
                HoraAtencion = reader["HoraAtencion"]?.ToString(),
                Consentimiento = reader["Consentimiento"]?.ToString(),
                PacienteNombreFirma = reader["PacienteNombreFirma"]?.ToString(),
                PacienteDocFirma = reader["PacienteDocFirma"]?.ToString(),
                RepNombre = reader["RepNombre"]?.ToString(),
                RepDoc = reader["RepDoc"]?.ToString(),
                FirmaPac = reader["FirmaPac"]?.ToString(),
                FirmaRep = reader["FirmaRep"]?.ToString(),
                CreadoAt = reader.GetDateTime(reader.GetOrdinal("CreadoAt"))
            };

        }

        return null;

    }
}
