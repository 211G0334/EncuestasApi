using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EncuestasApi.Models.Entities;

public partial class EncuestasContext : DbContext
{
    public EncuestasContext()
    {
    }

    public EncuestasContext(DbContextOptions<EncuestasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumnos> Alumnos { get; set; }

    public virtual DbSet<Encuestas> Encuestas { get; set; }

    public virtual DbSet<Encuestasaplicadas> Encuestasaplicadas { get; set; }

    public virtual DbSet<Preguntas> Preguntas { get; set; }

    public virtual DbSet<Respuestas> Respuestas { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=root;database=encuestas", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Alumnos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("alumnos");

            entity.Property(e => e.Nombre).HasMaxLength(75);
            entity.Property(e => e.NumeroControl)
                .HasMaxLength(8)
                .IsFixedLength();
        });

        modelBuilder.Entity<Encuestas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("encuestas");

            entity.HasIndex(e => e.UsuarioId, "fk_EncuestaUsuario");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.Titulo).HasMaxLength(200);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Encuestas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_EncuestaUsuario");
        });

        modelBuilder.Entity<Encuestasaplicadas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("encuestasaplicadas");

            entity.HasIndex(e => e.UsuarioId, "fk_AplicacioUsario");

            entity.HasIndex(e => e.AlumnoId, "fk_AplicacionAlumno");

            entity.HasIndex(e => e.EncuestaId, "fk_EncuestaAplicacion");

            entity.Property(e => e.FechaAplicacion).HasColumnType("datetime");

            entity.HasOne(d => d.Alumno).WithMany(p => p.Encuestasaplicadas)
                .HasForeignKey(d => d.AlumnoId)
                .HasConstraintName("fk_AplicacionAlumno");

            entity.HasOne(d => d.Encuesta).WithMany(p => p.Encuestasaplicadas)
                .HasForeignKey(d => d.EncuestaId)
                .HasConstraintName("fk_EncuestaAplicacion");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Encuestasaplicadas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_AplicacioUsario");
        });

        modelBuilder.Entity<Preguntas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("preguntas");

            entity.HasIndex(e => e.EncuestaId, "fk_EncuestaPregunta");

            entity.Property(e => e.Texto).HasMaxLength(200);

            entity.HasOne(d => d.Encuesta).WithMany(p => p.Preguntas)
                .HasForeignKey(d => d.EncuestaId)
                .HasConstraintName("fk_EncuestaPregunta");
        });

        modelBuilder.Entity<Respuestas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("respuestas");

            entity.HasIndex(e => e.EncuestaId, "fk_RescpuestaEncuesta");

            entity.HasOne(d => d.Encuesta).WithMany(p => p.Respuestas)
                .HasForeignKey(d => d.EncuestaId)
                .HasConstraintName("fk_RescpuestaEncuesta");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.Property(e => e.Contraseña)
                .HasMaxLength(12)
                .HasColumnName("contraseña");
            entity.Property(e => e.Nombre).HasMaxLength(75);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
