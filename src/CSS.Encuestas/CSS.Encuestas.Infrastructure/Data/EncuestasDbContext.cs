using CSS.Encuestas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSS.Encuestas.Infrastructure.Data;
public class EncuestasDbContext : DbContext
{
    public EncuestasDbContext(DbContextOptions<EncuestasDbContext> options) : base(options) { }

    public DbSet<Encuesta> Encuestas => Set<Encuesta>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Opcion> Opciones => Set<Opcion>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();
    public DbSet<RespuestaDetalle> RespuestasDetalle => Set<RespuestaDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Encuesta>().ToTable("Encuestas");
        modelBuilder.Entity<Pregunta>().ToTable("Preguntas");
        modelBuilder.Entity<Opcion>().ToTable("Opciones");
        modelBuilder.Entity<Respuesta>().ToTable("Respuestas");
        modelBuilder.Entity<RespuestaDetalle>().ToTable("RespuestasDetalle");

        modelBuilder.Entity<Pregunta>()
            .HasOne(p => p.Encuesta)
            .WithMany(e => e.Preguntas)
            .HasForeignKey(p => p.EncuestaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Opcion>()
            .HasOne(o => o.Pregunta)
            .WithMany(p => p.Opciones)
            .HasForeignKey(o => o.PreguntaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Respuesta>()
            .HasOne(r => r.Encuesta)
            .WithMany()
            .HasForeignKey(r => r.EncuestaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RespuestaDetalle>()
            .HasOne(d => d.Respuesta)
            .WithMany(r => r.Detalles)
            .HasForeignKey(d => d.RespuestaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RespuestaDetalle>()
            .HasOne(d => d.Pregunta)
            .WithMany()
            .HasForeignKey(d => d.PreguntaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RespuestaDetalle>()
            .HasOne(d => d.Opcion)
            .WithMany()
            .HasForeignKey(d => d.OpcionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
