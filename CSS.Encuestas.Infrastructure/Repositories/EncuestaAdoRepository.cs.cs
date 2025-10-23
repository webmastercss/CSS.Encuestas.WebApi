using CSS.Encuestas.Application.Interfaces;
using CSS.Encuestas.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

}
