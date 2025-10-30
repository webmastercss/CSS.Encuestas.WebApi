using CSS.Encuestas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.Infrastructure.Persistence;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EncuestaPreanalitica> Encuestas => Set<EncuestaPreanalitica>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<EncuestaPreanalitica>();
        e.ToTable("EncuestaPreanalitica");
        e.HasKey(x => x.Id);
        e.Property(x => x.Fecha).IsRequired();
        e.Property(x => x.NumeroRecepcion).HasMaxLength(100);
        e.Property(x => x.NombreCompleto).HasMaxLength(200);
        e.Property(x => x.Identificacion).HasMaxLength(50);
        e.Property(x => x.Edad).HasMaxLength(10);
        e.Property(x => x.Sexo).HasMaxLength(10);
        e.Property(x => x.Telefono).HasMaxLength(50);
        e.Property(x => x.Embarazada).HasMaxLength(50);
        e.Property(x => x.PeriodoMenstrual).HasMaxLength(50);
        e.Property(x => x.Ayuno).HasMaxLength(50);
        e.Property(x => x.SintomasRespiratorios).HasMaxLength(200);
        e.Property(x => x.Rechazo).HasMaxLength(50);
        e.Property(x => x.MotivoRechazo).HasMaxLength(200);
        e.Property(x => x.EnfermedadesBase).HasMaxLength(500);
        e.Property(x => x.Medicamentos).HasMaxLength(500);
        e.Property(x => x.ResponsableToma).HasMaxLength(150);
        e.Property(x => x.HoraAtencion).HasMaxLength(50);
        e.Property(x => x.Consentimiento).HasMaxLength(50);
        e.Property(x => x.PacienteNombreFirma).HasMaxLength(150);
        e.Property(x => x.PacienteDocFirma).HasMaxLength(50);
        e.Property(x => x.RepNombre).HasMaxLength(150);
        e.Property(x => x.RepDoc).HasMaxLength(50);
        e.Property(x => x.FirmaPac).HasMaxLength(200);
        e.Property(x => x.FirmaRep).HasMaxLength(200);
        e.Property(x => x.CreadoAt).HasDefaultValueSql("GETUTCDATE()");
    }
}