using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Encuestasaplicadas
{
    public int Id { get; set; }

    public DateTime? FechaAplicacion { get; set; }

    public int? AlumnoId { get; set; }

    public int? EncuestaId { get; set; }

    public int? UsuarioId { get; set; }

    public virtual Alumnos? Alumno { get; set; }

    public virtual Encuestas? Encuesta { get; set; }

    public virtual Usuarios? Usuario { get; set; }
}
