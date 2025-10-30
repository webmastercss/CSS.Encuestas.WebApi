using CSS.Encuestas.Application.Dtos.Encuesta;
using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Domain.Entities;
using CSS.Encuestas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.Infrastructure.Repositories;
public class EncuestaEfCoreRepository(AppDbContext db) : IEncuestaRepository
{

    public async Task<int> AddAsync(EncuestaPreanalitica entity)
    {
        db.Encuestas.Add(entity);
        await db.SaveChangesAsync();

        return entity.Id;


    }

    public Task<EncuestaPreanaliticaDto?> GetAsync(int id)
    {

        return db.Encuestas
    .AsNoTracking()
    .Select(e => new EncuestaPreanaliticaDto
    {
        Id = e.Id,
        NumeroRecepcion = e.NumeroRecepcion,
        NombreCompleto = e.NombreCompleto,
        Identificacion = e.Identificacion,
        Edad = e.Edad,
        Sexo = e.Sexo,
        Telefono = e.Telefono,
        Embarazada = e.Embarazada,
        PeriodoMenstrual = e.PeriodoMenstrual,
        Ayuno = e.Ayuno,
        SintomasRespiratorios = e.SintomasRespiratorios,
        Rechazo = e.Rechazo,
        MotivoRechazo = e.MotivoRechazo,
        EnfermedadesBase = e.EnfermedadesBase,
        Medicamentos = e.Medicamentos,
        ResponsableToma = e.ResponsableToma,
        HoraAtencion = e.HoraAtencion,
        Consentimiento = e.Consentimiento,
        PacienteNombreFirma = e.PacienteNombreFirma,
        PacienteDocFirma = e.PacienteDocFirma,
        RepNombre = e.RepNombre,
        RepDoc = e.RepDoc,
        FirmaPac = e.FirmaPac,
        FirmaRep = e.FirmaRep,
        CreadoAt = e.CreadoAt
    })
    .FirstOrDefaultAsync(e => e.Id == id);


    }
}
